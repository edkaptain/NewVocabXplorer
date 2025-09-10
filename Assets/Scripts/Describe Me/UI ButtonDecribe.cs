using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class UIButtonDescribe : MonoBehaviour
{
    public static event System.Action<string> OnImageSelected;

    [SerializeField]
    private string currentItemName;

    // Este botón también necesita saber qué objeto le corresponde
    [SerializeField]
    private int slotIndex = 0;

    void Start()
    {
        // Añade el evento al botón
        GetComponent<Button>().onClick.AddListener(() => OnButtonSelected(currentItemName));
    }

    private void OnEnable()
    {
        GameManagerDescribe.OnNewOrderSet += ChangeItemName;
    }

    private void OnDisable()
    {
        GameManagerDescribe.OnNewOrderSet -= ChangeItemName;
    }

    /// <summary>
    /// Cambia el nombre actual del objeto de este botón según el slotIndex
    /// </summary>
    private void ChangeItemName(List<GameObject> itemList)
    {
        if (slotIndex < itemList.Count)
        {
            var item = itemList[slotIndex];
            currentItemName = item.GetComponent<Interactableitem>().itemName;
            transform.GetChild(1).GetComponent<Text>().text = item.GetComponent<Interactableitem>().itemName;
        }
        else
        {
            Debug.LogWarning($"No se pudo asignar nombre al botón con slot {slotIndex} porque la lista tiene solo {itemList.Count} elementos.");
        }
    }


    /// <summary>
    /// Enviar el nombre del objeto cuando se hace clic
    /// </summary>
    public void OnButtonSelected(string itemName)
    {
        OnImageSelected?.Invoke(currentItemName);
    }
}
