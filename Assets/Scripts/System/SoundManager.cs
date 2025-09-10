using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        // Cachear el componente para evitar llamadas repetidas
        audioSource = GetComponent<AudioSource>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye este objeto duplicado
            return;
        }
        Instance = this;
    }

    public void PlaySound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            Debug.LogWarning("Sound name is null or empty.");
            return;
        }

        string path = $"Sounds/{soundName}";
        AudioClip clip = Resources.Load<AudioClip>(path);


        if (clip == null)
        {
            Debug.LogError($"Sound '{soundName}' not found in Resources folder.");
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}
