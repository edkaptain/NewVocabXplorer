using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Extensions;
public class RadialSelection : MonoBehaviour
{
    public OVRInput.Button spawnButton;

    int numberOfRadialPart;
    public List<GameObject> spawnedParts = new List<GameObject>();
    public Transform radialPartCanvas;
    public float angleBetweenRadials;

    public  UnityEvent<int> OnPartSelected;

    public Transform handTransform;
    int currentSelectedRadialPart = -1;

    private List<GameObject> selectedParts = new List<GameObject>();

    int lastSelected;
    float radio = 90;
    [SerializeField] private Transform radialDurationCanvas;
    [SerializeField] private Image progressRadialBar;
    private float time, duration;
    private bool isOnTimer;
    private void Start()
    {
        progressRadialBar.SetActive(false);
        numberOfRadialPart = spawnedParts.Count;
        NotificationsManager.Instance.OnAvatarVoiceDuration += Timer;
        NotificationsManager.Instance.PopUpNotification += NotificationPopUp;
    }

    private void Timer(float seconds)
    {
        progressRadialBar.fillAmount = 0;
        isOnTimer = true;
        duration = seconds;
        DurationRadialBar(true);
    }

    void Update()
    {
        if (isOnTimer) {
            time += Time.deltaTime;

            DurationRadialBar(true);
            progressRadialBar.fillAmount = time / duration;

            if (time > duration) {
                time = 0;
                isOnTimer = false;
                DurationRadialBar(false);
            }
        }

        if (OVRInput.GetDown(spawnButton))
        {
            SpawnRadialPart();
        }

        if (OVRInput.Get(spawnButton))
        {
            GetSelectedRadialPart();
        }

        if (OVRInput.GetUp(spawnButton))
        {
            HideAndTriggerSelected();
        }
    }


    private void DurationRadialBar(bool status) {
        
        radialDurationCanvas.gameObject.SetActive(status);
        progressRadialBar.SetActive(status);

        radialDurationCanvas.position = handTransform.position;
        radialDurationCanvas.rotation = handTransform.rotation;
    }
    public void HideAndTriggerSelected()
    {
        OnPartSelected?.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward);

        if (angle < 0)
        {
            angle += 360;
        }

        //Debug.Log($"Angle: {angle}");
        currentSelectedRadialPart = (int)angle * numberOfRadialPart / 360;
        //Debug.Log($"Selected Radial Part: {currentSelectedRadialPart}");

        if (currentSelectedRadialPart != lastSelected) PlayerController.Instance.VibrateController(OVRInput.Controller.RHand, .3f, .15f);
        for (int i = 0; i < spawnedParts.Count; i++)
        {
            Color black100 = new Color(0, 0, 0, .75f);
            Color original = spawnedParts[i].GetComponent<Image>().color;
            selectedParts[i].GetComponent<Image>().color = (i == currentSelectedRadialPart) ? black100 : original;
            selectedParts[i].transform.localScale = (i == currentSelectedRadialPart) ? 1.1f * Vector3.one : Vector3.one;
        }
        lastSelected = currentSelectedRadialPart;

    }
    private void SpawnRadialPart()
    {
        radialPartCanvas.gameObject.SetActive(true);
        radialPartCanvas.position = handTransform.position;
        radialPartCanvas.rotation = handTransform.rotation;

        float angleStep = 360f / numberOfRadialPart; // Separación entre objetos

        if (radialPartCanvas.childCount != spawnedParts.Count)
        {
            for (int i = 0; i < numberOfRadialPart; i++)
            {
                float angle = -i * angleStep + angleBetweenRadials; // Ángulo en el que se posicionará el objeto
                float radians = angle * Mathf.Deg2Rad; // Convertir a radianes

                // Calcular la posición en círculo manteniendo la misma distancia del origen
                Vector3 position = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * radio;

                // Instanciar el objeto
                GameObject spawnedRadialPart = Instantiate(spawnedParts[i], radialPartCanvas);

                // Ajustar la posición relativa al centro
                spawnedRadialPart.transform.localPosition = position;
                selectedParts.Add(spawnedRadialPart);
            }
        }
        

    }

    private void NotificationPopUp(bool status)
    {
        selectedParts[0].transform.GetChild(1).SetActive(!status);
    }



}
