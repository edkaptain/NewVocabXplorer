using UnityEngine;
using System.Collections;

public class Vibration : MonoBehaviour
{
    public static Vibration Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void Vibrate(float duration, OVRInput.Controller controller)
    {
        StartCoroutine(VibrateCoroutine(duration, controller));
    }

    private IEnumerator VibrateCoroutine(float duration, OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(0.1f, 0.1f, controller);

        yield return new WaitForSeconds(duration);

        OVRInput.SetControllerVibration(0f, 0f, controller);
    }
    /// <summary>
    /// Vibrates the VR Controllers
    /// </summary>
    /// <param name="duration"></param>
    public void Vibrate(float duration)
    {
        Vibrate(duration, OVRInput.Controller.RTouch);
        Vibrate(duration, OVRInput.Controller.LTouch);
    }
    private OVRInput.Controller GetGrabController()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            return OVRInput.Controller.RTouch;

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            return OVRInput.Controller.LTouch;

        return OVRInput.Controller.None;
    }

}