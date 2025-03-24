using UnityEngine;
using UnityEngine.Android;

public class MicGPTResponse
{
    private bool isRecording;
    public void MicrophonePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
    }

    public void StartRecording()
    {
        isRecording = true;

        var index = PlayerPrefs.GetInt("user-mic-device-index");

    }
}
