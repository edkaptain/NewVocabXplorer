using System;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
    //Singleton
    public static NotificationsManager Instance { get; private set; }
    private float timeDuration = 0;
    public bool isOnAvatar;
    private bool statusTime;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 🔹 Definir delegados y eventos
    public delegate void VoiceDurationHandler(float duration);
    public event VoiceDurationHandler OnAvatarVoiceDuration;


    public delegate void NotificationHandler(bool status);
    public event NotificationHandler PopUpNotification;

    // 🔹 Trigger events.
    public void NotifyAvatarVoiceDuration(float duration)
    {
        OnAvatarVoiceDuration?.Invoke(duration);
    }

    public void NotifyPopUpNotification(bool status) { 
    
        PopUpNotification?.Invoke(status);
    }
    //Check time duration.
    public void NotifyOnTimeDuration(bool status)
    {
        statusTime = status;
        if (!status)
        {
            Debug.Log("=== STATUS TIME === >> " + timeDuration + " << ===");

            timeDuration = 0;
        }
    }

    private void Update()
    {
        if (statusTime)
        {
            timeDuration += Time.deltaTime;
        }
    }

    public void PlayAudioClipNotification(string name)
    {
        try
        {
            if (!string.IsNullOrEmpty(name))
            {
                AudioClip clip = Resources.Load<AudioClip>(name);
                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                }
                else
                {
                    Debug.LogWarning($"No se encontró el AudioClip con el nombre: {name}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al intentar reproducir el audio '{name}': {ex.Message}");
        }
    }




}
