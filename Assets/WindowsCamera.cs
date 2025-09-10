using UnityEngine;
using UnityEngine.InputSystem;

public class WindowsCamera : MonoBehaviour
{
    private Camera main_camera;
    private CharacterController character_controller;
    //Vector que contiene el movimiento que genera
    private Vector2 moveInput;
    private bool fired;

    //Grupo que contiene los principales movimientos
    public InputActionReference move;
    public InputActionReference fire;

    public float move_Speed = 5f;

    private void Start()
    {
        main_camera = transform.Find("Camera").GetComponent<Camera>();
        character_controller = GetComponent<CharacterController>();
    }  

    private void Update()
    {
        moveInput = move.action.ReadValue<Vector2>();
        fired = fire.action.triggered;
        Vector3 direction = new Vector3(moveInput.x,0,moveInput.y);
        character_controller.Move(direction * move_Speed * Time.deltaTime);
    }

}
