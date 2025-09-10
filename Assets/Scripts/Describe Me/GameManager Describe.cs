using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerDescribe : MonoBehaviour
{
    public static GameManagerDescribe Instance;

    private int score, maxPoints;
    private string actualName;

    [SerializeField]
    private UIManagerDescribe UIManager;

    [SerializeField]
    private Avatar2 AvatarAssistant;

    public List<GameObject> itemList = new List<GameObject>();
    private List<GameObject> usedCorrectAnswers = new List<GameObject>();
    public static event Action<List<GameObject>> OnNewOrderSet;
    public static event Action<int> ClearGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        itemList = ItemListManager.Instance.ItemGameObjects;
        maxPoints = itemList.Count;
        UIManager.UpdateScore(score);
        //UpdateSystemCards();
    }

    private void OnEnable()
    {
        UIButtonDescribe.OnImageSelected += CheckImageMatch;
    }

    private void OnDisable()
    {
        UIButtonDescribe.OnImageSelected -= CheckImageMatch;
    }

    private void CheckImageMatch(string itemName)
    {
        Debug.Log($"Usuario seleccionó: {itemName}");

        if (actualName == itemName)
        {

            SoundManager.Instance.PlaySound("Correct Sound");
            score++;
            UIManager.UpdateScore(score);

            if (score >= maxPoints)
            {
                AvatarAssistant.AutoResponse($"System: The user has ended the activity.", false);
                UIManager.UpdateTextOne("Completed activity");
            }
            else if (score == 3)
            {
                AvatarAssistant.AutoResponse($"System: good response.", false);
                UpdateSystemCards();
            }
            else
            {
                UpdateSystemCards();
            }
        }
        else
        {
            SoundManager.Instance.PlaySound("Wrong Sound");
            Debug.Log("Incorrecto. Intenta de nuevo.");
        }
    }

    private List<GameObject> GetRandomItems()
    {
        List<GameObject> selectedItems = new List<GameObject>();// Es la lista que entrega los objetos
        List<int> indicesUsados = new List<int>(); // Lista para que no se repitan los objetos
        int correctIndex;

        do
        {
            selectedItems.Clear();
            indicesUsados.Clear();

            // Selecciona 3 índices únicos
            while (selectedItems.Count < 3)
            {
                int randIndex = UnityEngine.Random.Range(0, itemList.Count);
                if (!indicesUsados.Contains(randIndex))
                {
                    selectedItems.Add(itemList[randIndex]);
                    indicesUsados.Add(randIndex);
                }
            }

            // Elegir uno como correcto
            correctIndex = UnityEngine.Random.Range(0, 3); // índice en selectedItems

        } while (usedCorrectAnswers.Contains(selectedItems[correctIndex])); 
        //Mientras ya haya seleccionado anteriormente el objeto correcto, se seguirá repitiendo hasta que encuentré otro diferente.

        // Guardar el correcto
        GameObject actualCorrectItem = selectedItems[correctIndex];
        usedCorrectAnswers.Add(actualCorrectItem);

        // Guarda el string de la respuesta correcta
        actualName = actualCorrectItem.GetComponent<Interactableitem>().itemName;

        UIManager.CorrectAnswer(actualName);
        AvatarAssistant.AutoResponse($"System: currentItem({actualName})", false);

        return selectedItems;
    }



    public void UpdateSystemCards()
    {
        // Obtener hasta 3 objetos disponibles
        List<GameObject> randomItems = GetRandomItems();

        if (randomItems != null && randomItems.Count > 0)
        {
            OnNewOrderSet?.Invoke(randomItems);            
        }
        else
        {
            Debug.LogWarning("No hay suficientes objetos para continuar.");
        }
    }

    public void ClearSystem()
    {
        score = 0;
        UpdateSystemCards();
        AvatarAssistant.ClearChat();
        ClearGame?.Invoke(score);
    }
}
