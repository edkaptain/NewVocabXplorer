using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_repeat;
    [SerializeField] private Button btn_clear;
    [SerializeField] private Text txtbtn;
    public UnityEvent <int> menuEvents;    

    private bool start_status;
    private void Start()
    {
        btn_start.onClick.AddListener(OnStart);
        btn_repeat.onClick.AddListener(() => OnButtonPressed(1));
    }

    private void OnButtonPressed(int id) // Selecciona la accion del avatar a comenzar por ejemplo 0 -> Start recording, 1 Repeat voice 
    {
        if(menuEvents != null)
        {
            menuEvents.Invoke(id);
        }
    }
    private void OnStart()
    {
        if (start_status == false)
        {
            //Stop button
            OnButtonPressed(0);
            txtbtn.GetComponent<Text>().text = "Please click here to stop recording";
            btn_start.GetComponentInChildren<Text>().text = "Stop";
            btn_start.GetComponent<Image>().color = Color.red;
            btn_start.GetComponentInChildren<Text>().color = Color.white;
            start_status = true;
        }
        else
        {
            //Normal button
            OnButtonPressed(4);
            txtbtn.GetComponent<Text>().text = "Click here to start a conversation";
            btn_start.GetComponentInChildren<Text>().text = "Start";
            btn_start.GetComponent<Image>().color = Color.white;
            btn_start.GetComponentInChildren<Text>().color = Color.black;
            start_status = false;
        }

    }
}
