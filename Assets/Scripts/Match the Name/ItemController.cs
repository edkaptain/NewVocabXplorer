using UnityEngine;

public class ItemController : MonoBehaviour
{
    // Evento estático para notificar la selección de un item.
    public static event System.Action<string> OnItemSelected;
   
    /// <summary>
    /// Invoke the item
    /// </summary>
    /// <param name="itemName"></param>
    public void OnItemName(string itemName) {
        OnItemSelected?.Invoke(itemName);
    }
}
