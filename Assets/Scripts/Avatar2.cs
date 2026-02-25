using ElevenLabs;
using ElevenLabs.Models;
using ElevenLabs.TextToSpeech;
using ElevenLabs.Voices;
using OpenAI;
using Samples.Whisper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

public class Avatar2 : MonoBehaviour
{
    [Header("Characteristics")]
    //[SerializeField, TextArea(2, 3)] private string avatarActitude;
    [SerializeField] private TextAsset avatarMetaPrompt; //Pending

    [Header("Voice Configuration")]
    private ElevenLabsConfiguration configuration;
    [SerializeField] private Voice voice;
    [SerializeField] private AudioSource audioSource;
    private AudioClip audioClip;

    [Header("GPT Menu")]
    [SerializeField] private GameObject ChatGPTCanvas;
    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    private string GPTResponse;
    private readonly string fileName = "output.wav";
    private int prompt_count = 0;
    

    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
        StartChat();       
    }

    //Actions connected through a menu interactable.
    public async void Actions(int idAction)
    {
        switch (idAction)
        {
            case 0: // Voice input button
                StartRecording();
                break;

            case 1: // Retry button
                PlayMessage(GPTResponse);
                break;

            case 2: // Clear button
                openAI = null;
                openAI = new OpenAIApi();
                messages.Clear();
                audioClip = null;

                break;
            case 4: // End Microphone
                await EndRecording();
                await ChatGPT(GPTResponse,true);
                PlayMessage(GPTResponse);
                break;
            default:
                UnityEngine.Debug.LogWarning("Action no valid");
                break;

        }
    }

    // *--- Recording ---*
    private void StartRecording()
    {
        UnityEngine.Debug.Log("*--- Start recording ---*");
        GPTResponse = string.Empty;
        audioClip = Microphone.Start(null, false, 30, 44100);

    }
    private async Task EndRecording()
    {
        ChatGPTCanvas.GetComponent<ChatGPTcontroller>().StartChatWait(true);
        Microphone.End(null);
        byte[] data = SaveWav.Save(fileName, audioClip);

        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            Model = "whisper-1",
            Language = "en"
        };

        var res = await openAI.CreateAudioTranscription(req);
        UnityEngine.Debug.Log("User: " + res);
        GPTResponse = res.Text;
    }

    // *--- ChatGPT ---*
    public async Task ChatGPT(string Content,bool appendMessages)
    {
        GPTResponse = "";
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = Content
        };


        // First Response
        if (messages.Count == 0)
        {
            //newMessage.Content = "\n" + newMessage.Content;
            string extraContentPrompt = avatarMetaPrompt?.text ?? string.Empty;
            newMessage.Content = $" /n {newMessage.Content} /n This is the JSON file to follow and act as this tells you /n {extraContentPrompt}";
        }
        else {
            if (appendMessages)
            {
                ChatGPTCanvas.GetComponent<ChatGPTcontroller>().AppendMessage(newMessage);
            }
            ChatGPTCanvas.GetComponent<ChatGPTcontroller>().StartChatWait(true);
        }

        UnityEngine.Debug.Log("Message: " + messages.Count.ToString() + " " + newMessage.Content);
        messages.Add(newMessage);

        var completionResponse = await openAI.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            //Model = "gpt-4o",
            Model = "gpt-4o",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            prompt_count += 1 ;
            var message = completionResponse.Choices[0].Message;
            GPTResponse = message.Content?.Trim() ?? "No GPTResponse received.";
            UnityEngine.Debug.Log($"Response {prompt_count}: ${GPTResponse}");
            ChatGPTCanvas.GetComponent<ChatGPTcontroller>().AppendMessage(message);
            
        }
        else
        {
            GPTResponse = "No GPTResponse received.";
            UnityEngine.Debug.LogWarning("No PrintText was generated from this prompt.");
        }

        ChatGPTCanvas.GetComponent<ChatGPTcontroller>().StartChatWait(false);

    }

    // *--- Avatar Voice ---*
    private async void PlayMessage(string message)
    {
        try
        {
            var api = new ElevenLabsClient(configuration);

            if (voice == null)
            {
                voice = (await api.VoicesEndpoint.GetAllVoicesAsync(destroyCancellationToken)).FirstOrDefault();
            }

            var request = new TextToSpeechRequest(voice, message, model: Model.FlashV2_5, outputFormat: OutputFormat.PCM_24000);
            var stopwatch = Stopwatch.StartNew();
            audioClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(request, cancellationToken: destroyCancellationToken);
            var elapsedTime = (float)stopwatch.Elapsed.TotalSeconds;
            //var playbackTime = audioClip.length - elapsedTime;
            audioSource.clip = audioClip;
            //await Task.Delay(TimeSpan.FromSeconds(playbackTime + 1f), destroyCancellationToken);            
            audioSource.Play();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="showMesagges">Will show the messages in the chat</param>
    public async void AutoResponse(string message, bool showMesagges)
    {
        GPTResponse = message;
        await ChatGPT(GPTResponse,showMesagges);
        PlayMessage(GPTResponse);
    }
    private async void StartChat()
    {
        await ChatGPT(string.Empty,false);
        ChatGPTCanvas.GetComponent<ChatGPTcontroller>().ClearContent();
    }

    public void ClearChat()
    {
        ChatGPTCanvas.GetComponent<ChatGPTcontroller>().ClearContent();
        messages.Clear();
        openAI = new OpenAIApi();
        GPTResponse = string.Empty;
        audioClip = null;
        StartChat();
    }

}
