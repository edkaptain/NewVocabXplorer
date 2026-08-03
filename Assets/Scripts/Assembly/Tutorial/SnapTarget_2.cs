using UnityEngine;
using UnityEngine.Events;

public class SnapTarget_2 : MonoBehaviour
{
    public UnityEvent onAction;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning($"The object {other} has entered");
        SnapObject_2 snapObject = other.GetComponent<SnapObject_2>();

        if (snapObject == null)
            return;

        snapObject.Snap(gameObject);
        onAction?.Invoke();
        gameObject.SetActive(false);
    }

    

}
