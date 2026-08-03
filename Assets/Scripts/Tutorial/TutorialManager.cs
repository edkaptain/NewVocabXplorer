using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [Header("Testing")]
    public Light environmentLight;
    public TutorialVoiceManager voiceManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetSlides();
        ColorUtility.TryParseHtmlString("#FFF4D6", out Color warmColor);
        environmentLight.color = warmColor;
    }

    public GameObject canvas;
    public List<GameObject> slides = new List<GameObject>();
    public int current = 0;

    private void OnValidate()
    {
        if (canvas != null)
        {
            SlidesFinder(canvas);
        }
    }

    private void SlidesFinder(GameObject gameObject)
    {
        slides.Clear();

        foreach (Transform child in gameObject.transform)
        {
            slides.Add(child.gameObject);
        }

        slides.RemoveAt(slides.Count - 1);
    }

    [ContextMenu("Reset all sildes")]
    public void ResetSlides()
    {
        foreach (GameObject child in slides)
        {
            child.SetActive(false);
        }

        ChangeLight();
        slides[0].SetActive(true);
        current = 0;

    }

    [ContextMenu("Change the next slide")]
    public void ChangeNextSlide()
    {
        if (current != slides.Count - 1) {
            slides[current].gameObject.SetActive(false);

            slides[current + 1].gameObject.SetActive(true);
            current++;

            if (voiceManager.audiosource.isPlaying)
            {
                voiceManager.audiosource.Stop();
            }
            
            ChangeLight();
        }
        else
        {
            ResetSlides();
            current = 0;
        }        
    }


    public void ChangeLight()
    {
        int[] arr = { 2,3,5};

        ColorUtility.TryParseHtmlString("#FFF4D6", out Color warmColor);
        ColorUtility.TryParseHtmlString("#D6FBFF", out Color coldColor);

        bool isInArray = false;

        for (int i = 0; i < arr.Length; i++)
        {
            if (current == arr[i])
            {
                isInArray = true;
                break;
            }
        }

        environmentLight.color = isInArray ? coldColor : warmColor;
    }
}
