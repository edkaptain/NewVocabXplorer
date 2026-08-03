using UnityEngine;
using UnityEngine.UI;

public class DetectUser : MonoBehaviour
{
    public MoveTutorial moveTutorial;
    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used)
            return;

        used = true;

        transform.parent.gameObject.SetActive(false);

        moveTutorial.AddPoints();

        AudioManager.Instance.SystemNotification(true);
    }
}