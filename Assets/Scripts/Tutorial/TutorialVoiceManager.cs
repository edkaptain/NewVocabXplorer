using System.Collections;
using UnityEngine;

public class TutorialVoiceManager : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip[] AudioClip;

    private void OnValidate()
    {
        if (audiosource != null) {
            audiosource = gameObject.GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        StartCoroutine(Delay());
    }

    public void PlayVoiceAudio(int voice)
    {
        if (audiosource != null) {
            audiosource.PlayOneShot(AudioClip[voice]);
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(5f);
        audiosource.PlayOneShot(AudioClip[0]);

    }
}
