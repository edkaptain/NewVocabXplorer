using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemListManager : MonoBehaviour
{
    public static ItemListManager Instance;

    public List<string> ItemListInTable = new List<string>();
    public List<GameObject> ItemGameObjects = new List<GameObject>();

    //Evento unico de la lista por si requiere se llamado
    //public event Action<List<string>> OnItemListChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye este objeto duplicado
            return;
        }
        Instance = this;


        CheckCurrentItems();
    }

    private void CheckCurrentItems()
    {
        ItemListInTable.Clear();
        ItemGameObjects.Clear();

        GameObject[] items = GameObject.FindGameObjectsWithTag("TableItems");

        GameObject[] gameObjectItems = GameObject.FindGameObjectsWithTag("Item");
        HashSet<string> uniqueNames = new HashSet<string>();

        foreach (var item in items)
        {
            var controller = item.GetComponent<ItemController>();
            var interactable = item.GetComponent<Interactableitem>();

            if (controller != null && interactable != null)
            {
                ItemListInTable.Add(interactable.itemName);
            }
            else
            {
                Debug.LogWarning($"No se agregó el ítem {item.name} porque falta un componente.");
            }
        }
        //Checar
        foreach (var item in gameObjectItems)
        {
            var interactable = item.GetComponent<Interactableitem>();

            if (interactable != null)
            {
                string name = interactable.itemName;

                if (!uniqueNames.Contains(name))
                {
                    uniqueNames.Add(name);
                    ItemGameObjects.Add(item);
                }
            }
        }

        // Elimina duplicados
        ItemListInTable = ItemListInTable.Distinct().ToList();

        if (ItemListInTable.Count == 0 || ItemGameObjects.Count == 0)
        {
            Debug.LogWarning("La lista de items está vacía después de la verificación.");
        }
        else
        {
            Debug.Log("---Items: " + string.Join(", ", ItemListInTable));
        }

        // Opcional: notificar a otros sistemas
        //OnItemListChanged?.Invoke(ItemListInTable);
    }



}
