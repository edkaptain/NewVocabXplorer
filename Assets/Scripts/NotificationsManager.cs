using System;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
    //Singleton
    public static NotificationsManager Instance { get; private set; }

    public bool isOnAvatar;
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
    

    // 🔹 Métodos para disparar los eventos
    public void NotifyAvatarVoiceDuration(float duration)
    {
        OnAvatarVoiceDuration?.Invoke(duration);
    }

    public void NotifyPopUpNotification(bool status) { 
    
        PopUpNotification?.Invoke(status);
    }

    

  
}
