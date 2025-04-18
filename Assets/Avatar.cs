﻿using ElevenLabs;
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
using Debug = UnityEngine.Debug;

public class Avatar : MonoBehaviour
{
    [Header("Characteristics")]
    [SerializeField] private string avatarName;
    [SerializeField] private int avatarAge;
    [SerializeField, TextArea(2, 3)] private string avatarActitude;
    [SerializeField] private TextAsset avatarMetaPrompt; //Pending

    [Header("Voice Configuration")]
    private ElevenLabsConfiguration configuration;
    [SerializeField] private Voice voice;
    [SerializeField] private AudioSource audioSource;

    [Header("Debug Info")]
    [SerializeField] private GameObject chatWait;
    [SerializeField] private GameObject ChatGPTCanvas;
    private string response;
    private bool isRecording;
    [SerializeField, Range(2, 10)] private int duration = 10;
    private AudioClip clip;
    private readonly string fileName = "output.wav";
    private OpenAIApi openAI = new OpenAIApi();

    private List<ChatMessage> messages = new List<ChatMessage>();
    private AudioClip audioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NotificationsManager.Instance.isOnAvatar = true;
            NotificationsManager.Instance.NotifyPopUpNotification(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NotificationsManager.Instance.isOnAvatar = false;
            NotificationsManager.Instance.NotifyPopUpNotification(false);
        }
    }


    public async void Actions(int idAction)
    {
        switch (idAction)
        {
            case 0: // Voice input button
                if (NotificationsManager.Instance.isOnAvatar)
                {
                    StartToTalk();
                }
                else if (!NotificationsManager.Instance.isOnAvatar)
                {
                    Debug.Log("Error");
                    NotificationsManager.Instance.PlayAudioClipNotification("Error");
                }
                break;
            
            case 1: // Retry button
                if (NotificationsManager.Instance.isOnAvatar)
                {
                    audioSource.PlayOneShot(audioClip);
                }
                break;

            case 3: // Cancel button
               
                break;
            default:
                Debug.LogWarning("No valid");
                break;

        }
    }
    /// <summary>
    /// This method enables the voice input process with  an avatar.
    /// </summary>
    public async void StartToTalk()
    {
        NotificationsManager.Instance.NotifyOnTimeDuration(true);
        //Voice Input Text
        NotificationsManager.Instance.PlayAudioClipNotification("notification");
        await GeneratePrompt(await StartRecording());
        PlayMessage(response);
        NotificationsManager.Instance.NotifyOnTimeDuration(false);
    }

    private async Task<String> StartRecording()
    {
        response = string.Empty;
        NotificationsManager.Instance.NotifyAvatarVoiceDuration(duration);
        await WaitResponseAsync(duration);
        clip = Microphone.Start(null, false, duration, 44100);
        return response;
    }

    private async Task WaitResponseAsync(int seconds)
    {
        Debug.Log("Start Recording");
        isRecording = true;

        clip = Microphone.Start(null, false, seconds, 44100);
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        isRecording = false;
        await EndRecording();
    }

    private async Task EndRecording()
    {
        chatWait.SetActive(true);
        chatWait.GetComponent<Animator>().SetBool("status", true);
        Microphone.End(null);

        NotificationsManager.Instance.PlayAudioClipNotification("notification");

        byte[] data = SaveWav.Save(fileName, clip);

        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            Model = "whisper-1",
            Language = "en"
        };

        var res = await openAI.CreateAudioTranscription(req);
        Debug.Log("User: " + res);
        response = res.Text;


        chatWait.SetActive(false);
        chatWait.GetComponent<Animator>().SetBool("status", false);
        //SendReply(res.Text);
    }

    public async Task ChatGPT(string Content)
    {
        response = "";
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = Content
        };

        if (messages.Count == 0) newMessage.Content = "\n" + newMessage.Content;

        ChatGPTCanvas.GetComponent<ChatGPTcontroller>().AppendMessage(newMessage);

        Debug.Log("Message: " + messages.Count.ToString() + " " + newMessage.Content);
        messages.Add(newMessage);

        var completionResponse = await openAI.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-4o",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            response = message.Content?.Trim() ?? "No response received.";
            Debug.Log("Response: " + response);
            ChatGPTCanvas.GetComponent<ChatGPTcontroller>().AppendMessage(message);
        }
        else
        {
            response = "No response received.";
            Debug.LogWarning("No PrintText was generated from this prompt.");
        }
    }

    /// <summary>
    /// Generate the output response from ChatGPT
    /// </summary>
    /// <param name="content">String input</param>
    /// <returns>Response as string</returns>
    public async Task<string> GeneratePrompt(string content)
    {
        await ChatGPT(content); // Espera a que termine antes de continuar
        return response; // Ahora response ya tiene el valor correcto
    }

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
            Debug.LogException(e);
        }
    }
}