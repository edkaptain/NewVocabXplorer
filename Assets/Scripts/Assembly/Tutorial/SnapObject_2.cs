using Unity.Mathematics;
using UnityEngine;

public class SnapObject_2 : MonoBehaviour
{
    [Header("Testing")]
    public bool isSelected;
    public bool isLocked;
    public GameObject interaction;

    private Vector3 lockedPosition;
    private Quaternion lockedRotation;

    private void Update()
    {
        if (isLocked)
        {
            transform.SetPositionAndRotation(lockedPosition, lockedRotation);
        }
    }

    public void Snap(GameObject target)
    {
        LockPosition(target);
        Debug.LogWarning("The object is attached");
        
        
    }

    public void UpdateSelected(bool status)
    {
        isSelected = status;
    }

    public void LockPosition(GameObject target)
    {
        isLocked = true;
        interaction.SetActive(false);
        lockedPosition = target.transform.position;
        lockedRotation = target.transform.rotation;
        transform.SetPositionAndRotation(lockedPosition, lockedRotation);
    }
}
