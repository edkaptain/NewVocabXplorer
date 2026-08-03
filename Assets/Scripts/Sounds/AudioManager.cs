using Meta.WitAi;
using Oculus.VoiceSDK.UX;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    [Header("Sounds")]
    [Tooltip("Plays the Assembly Click sound")]
    [SerializeField] private AudioClip assemblyAttach;
    [SerializeField] private AudioClip[] systemNotification;
    [SerializeField] private AudioClip drop;
    [SerializeField] private AudioClip buzzerTone;
    [SerializeField] private AudioClip closeBox;
    [SerializeField] private AudioClip success;
    // Singlenton
    public static AudioManager Instance;

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Click()
    {
        if(audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(assemblyAttach);
        }
    }

    public void SystemNotification(bool Start)
    {
        if (Start)
        {
            audioSource.PlayOneShot(systemNotification[0]);
        }
        else
        {
            audioSource.PlayOneShot(systemNotification[1]);
        }
    }

    public void DropSound()
    {
        audioSource.PlayOneShot(drop);
    }

    public void Buzzer()
    {
        audioSource.PlayOneShot(buzzerTone);
    }

    public void CloseBox()
    {
        audioSource.PlayOneShot(closeBox);
    }

    public void Sucess()
    {
        audioSource.PlayOneShot(success);
    }
}
