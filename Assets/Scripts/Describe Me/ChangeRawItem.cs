using System.Collections.Generic;
using UnityEngine;

public class ChangeRawItem : MonoBehaviour
{
    [SerializeField]
    private int slotIndex = 0;
    private Vector3 oldPosition;

    [SerializeField]
    private float speed = 1f;

    private float padding = 1.5f; // Espacio extra alrededor del objeto
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        GameManagerDescribe.OnNewOrderSet += ChangeItemRawTexture;
    }
    private void OnDisable()
    {
        GameManagerDescribe.OnNewOrderSet -= ChangeItemRawTexture;
    }

    private void Update()
    {
        Transform container = transform.Find("GameObject");

        if (container != null && container.childCount > 0)
        {
            container.GetChild(0).Rotate(0f, Time.deltaTime * speed, 0f);
        }

    }

    private void ChangeItemRawTexture(List<GameObject> list)
    {
        GameObject itemList = list[slotIndex];

        Transform parent = transform.Find("GameObject");
        if (parent == null)
        {
            Debug.LogWarning("No se encontró el objeto 'GameObject' como hijo.");
            return;
        }

        if (parent.childCount != 0) {
            // Hay un hijo, se quita y se reemplaza
            Transform currentChild = parent.GetChild(0);
            currentChild.position = oldPosition;
            currentChild.SetParent(null, false);
            currentChild.GetComponent<Interactableitem>().ViewMaterial(false);
        }

        oldPosition = itemList.transform.position;
        itemList.transform.SetParent(parent, false);
        itemList.GetComponent<Interactableitem>().ViewMaterial(true);
        itemList.transform.SetPositionAndRotation(parent.position, Quaternion.identity);
        AttachCameraCenter(itemList.transform);
    }

    private void AttachCameraCenter(Transform targetObject)
    {
        Camera targetCamera = transform.Find("Camera").GetComponent<Camera>();

        Bounds bounds = GetBounds(targetObject);
        float radius = bounds.extents.magnitude;
        float fov = targetCamera.fieldOfView * Mathf.Deg2Rad;

        // Distancia necesaria según tamaño del objeto
        float distance = radius / Mathf.Tan(fov / 2f);

        targetCamera.orthographic = true;
        targetCamera.orthographicSize = bounds.extents.magnitude * padding;

        Quaternion rotation = Quaternion.Euler(30f, 45f, 0f); // X = inclinación, Y = giro
        targetCamera.transform.rotation = rotation;
        targetCamera.transform.position = bounds.center - rotation * Vector3.forward * distance * padding;

        targetCamera.transform.LookAt(bounds.center);

    }

    Bounds GetBounds(Transform target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(target.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }
        return bounds;
    }


}
