using ElevenLabs;
using ElevenLabs.Models;
using ElevenLabs.TextToSpeech;
using ElevenLabs.Voices;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class PronunceItem : MonoBehaviour
{
    public static PronunceItem Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("Voice Configuration")]
    private ElevenLabsConfiguration configuration;
    [SerializeField] private Voice voice;
    [SerializeField] private AudioSource audioSource;
    private AudioClip audioClip;

    public async void PlayMessage(string message)
    {
        if (!AudioExists(message))
        {
            try
            {
                var api = new ElevenLabsClient(configuration);

                if (voice == null)
                    voice = (await api.VoicesEndpoint.GetAllVoicesAsync(destroyCancellationToken)).FirstOrDefault();

                var request = new TextToSpeechRequest(voice, message, model: Model.FlashV2_5, outputFormat: OutputFormat.PCM_24000);
                audioClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(request, cancellationToken: destroyCancellationToken);

                audioSource.clip = audioClip;
                audioSource.Play();

                SaveWav(audioClip, message);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
        {
            await ReproducirDesdeArchivo(message);
        }
    }

    private async Task ReproducirDesdeArchivo(string fileName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ItemsName");
        string fullPath = Path.Combine(folderPath, fileName + ".wav");

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + fullPath, AudioType.WAV))
        {
            var request = www.SendWebRequest();
            while (!request.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Error al cargar audio desde archivo: " + www.error);
            }
        }
    }

    public static void SaveWav(AudioClip clip, string fileName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ItemsName");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, fileName + ".wav");
        Debug.Log("Guardando audio en: " + filePath);

        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fileStream))
        {
            int sampleCount = samples.Length;
            int byteRate = clip.frequency * clip.channels * 2;

            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + sampleCount * 2);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)clip.channels);
            writer.Write(clip.frequency);
            writer.Write(byteRate);
            writer.Write((short)(clip.channels * 2));
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(sampleCount * 2);

            foreach (float sample in samples)
            {
                short intSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                writer.Write(intSample);
            }
        }
    }

    public static bool AudioExists(string fileName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ItemsName");
        string fullPath = Path.Combine(folderPath, fileName + ".wav");
        return File.Exists(fullPath);
    }
}
