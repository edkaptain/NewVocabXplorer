using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RayInteractable), typeof(InteractableUnityEventWrapper), typeof(ColliderSurface))]
[RequireComponent(typeof(Outline))]
public class Interactableitem : MonoBehaviour
{
    [Header("Item Characteristics")]
    [SerializeField]
    public string itemName;

    private Material material;
    private Material OriginalMaterial;

    private GameObject canvas;
    private void Awake()
    {
        if (GetComponent<ItemController>() == null)
        {
            material = Resources.Load<Material>("Default");
            OriginalMaterial = GetComponent<Renderer>().material;
            gameObject.GetComponent<Renderer>().material = material;

            CanvasSetup();
        }
        OutlineSetup();
        InjectProperties();
        AddEvents();

    }

    private void CanvasSetup()
    {
        canvas = transform.GetChild(0).gameObject;


        if (itemName == string.Empty)
        {
            itemName = "No name assigned";
        }


        canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = itemName;

        CanvasSetUp(false);


    }

    private void OutlineSetup()
    {
        GetComponent<Outline>().OutlineColor = Color.yellow;
        GetComponent<Outline>().OutlineWidth = 2;
    }

    private void InjectProperties()
    {
        GetComponent<InteractableUnityEventWrapper>().InjectInteractableView(GetComponent<RayInteractable>());

        if (GetComponent<Collider>() == null || GetComponent<BoxCollider>())
        {
            gameObject.AddComponent<BoxCollider>();
        }
        GetComponent<ColliderSurface>().InjectCollider(GetComponent<Collider>());

        GetComponent<RayInteractable>().InjectSurface(GetComponent<ColliderSurface>());
    }

    private void AddEvents()
    {
        GetComponent<InteractableUnityEventWrapper>().WhenHover.AddListener(OnHover);
        GetComponent<InteractableUnityEventWrapper>().WhenUnhover.AddListener(UnHover);
        if (GetComponent<ItemController>())
        {
            GetComponent<InteractableUnityEventWrapper>().WhenSelect.AddListener(UIMatch);
        }
        else
        {

            GetComponent<InteractableUnityEventWrapper>().WhenSelect.AddListener(PronunceName);
            GetComponent<InteractableUnityEventWrapper>().WhenSelect.AddListener(SetMaterialDefault);
        }
    }

    private void UIMatch()
    {
        GetComponent<ItemController>().OnItemName(itemName);
    }

    private void OnHover()
    {
        CanvasSetUp(true);
        GetComponent<Outline>().IsEnabled = true;
    }

    private void UnHover()
    {
        CanvasSetUp(false);
        GetComponent<Outline>().IsEnabled = false;
    }
    private void CanvasSetUp(bool status)
    {
        if (canvas != null)
        {
            canvas.transform.LookAt(Camera.main.transform);
            canvas.transform.Rotate(0, 180, 0);
            canvas?.SetActive(status);
        }
        else
        {
            Debug.Log("No canvas added");
        }
    }

    private void PronunceName()
    {
        if (itemName != null)
        {
            PlayerController.Instance.VibrateController(OVRInput.Controller.Gamepad, .5f, .1f);
            PronunceItem.Instance.PlayMessage(itemName);
        }

    }
    private void SetMaterialDefault()
    {
        gameObject.GetComponent<Renderer>().material = OriginalMaterial;
    }

    public void ViewMaterial(bool status)
    {
        if (!status){
            material = Resources.Load<Material>("Default");
            OriginalMaterial = GetComponent<Renderer>().material;
            gameObject.GetComponent<Renderer>().material = material;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = OriginalMaterial;
        }
    }
}
