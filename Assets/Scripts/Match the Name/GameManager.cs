using ElevenLabs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private string actualName;
    private int score = 0;
    private int maxPoints = 0;
    private string NewItem = "";

    [SerializeField]
    private UIManager UIMatch;
    private Avatar2 Avatar;


    public List<string> itemList = new List<string>();
    List<int> newItems = new List<int>();
    private void Awake()
    {
        // Verifica si ya existe otra instancia y destruye esta si es necesario.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Evita duplicados.
            return;
        }

        // Asigna esta instancia como la única.
        Instance = this;

        // (Opcional) Si quieres que este objeto persista entre escenas:
        // DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {        
        itemList = ItemListManager.Instance.ItemListInTable;
        maxPoints = itemList.Count;
        UIMatch.UpdateScore(score);
        UIMatch.UpdateTextItem(GetNewItem());
    }

    private void OnEnable()
    {
        ItemController.OnItemSelected += CheckItem;
    }
    private void OnDisable()
    {
        ItemController.OnItemSelected -= CheckItem;
    }

    private void CheckItem(string itemName)
    {
        actualName = UIMatch.itemNameText.text;
        // Verifica si el nombre seleccionado es igual al nombre actual.
        if (actualName == itemName)
        {

            SoundManager.Instance.PlaySound("Correct Sound");
            // Suma puntos y actualiza el marcador.
            score++;
            UIMatch.UpdateScore(score);

            // Verifica si ya se alcanzaron los puntos máximos.
            if (score >= maxPoints)
            {

                UIMatch.UpdateTextItem("You've completed this activity! 😊");
            }
            else
            {

                // Actualiza el ítem en pantalla con un nuevo ítem.
                UIMatch.UpdateTextItem(GetNewItem());
            }
        }
        else
        {

            SoundManager.Instance.PlaySound("Wrong Sound");
            // Opcional: agregar feedback si el nombre no coincide.
            Debug.Log("Nombre incorrecto, intenta de nuevo.");
        }
    }
    private string GetNewItem()
    {      

        int randItem;

        // 1️⃣ Genera un número aleatorio único.
        do
        {
            randItem = Random.Range(0, itemList.Count);
        } while (newItems.Contains(randItem) && newItems.Count < itemList.Count);

        // 2️⃣ Añade el índice a la lista de ya usados.
        newItems.Add(randItem);

        // 3️⃣ Obtiene el item y lo devuelve.
        NewItem = itemList[randItem];
        return NewItem;
    }

}
