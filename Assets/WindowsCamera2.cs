using UnityEngine;
using UnityEngine.InputSystem;

public class WindowsCamera2 : MonoBehaviour
{
    private Camera main_camera;
    private CharacterController character_controller;

    private PlayerInput player_Input;
    private Vector2 input;
    private float rotation_Input;
    private float vertical_Input;

    public float move_Speed = 5f;
    public float rotation_Speed = 100f; // grados por segundo
    public float vertical_Speed = 5f;

    private void Start()
    {
        player_Input = GetComponent<PlayerInput>();
        main_camera = transform.Find("Camera").GetComponent<Camera>();
        character_controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        input = player_Input.actions["Move"].ReadValue<Vector2>();
        rotation_Input = player_Input.actions["Rotate"].ReadValue<float>(); // E = +1, Q = -1
        vertical_Input = player_Input.actions["Up&Down"].ReadValue<float>();

    }

    private void FixedUpdate()
    {
        // Movimiento
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        if (Mathf.Abs(vertical_Input) > 0.01f)
        {
            float verticalAmout = vertical_Input * vertical_Speed * Time.fixedDeltaTime;
            moveDirection = new Vector3(input.x, verticalAmout, input.y);
        }

        Vector3 worldMove = transform.TransformDirection(moveDirection);
        character_controller.Move(worldMove * move_Speed * Time.deltaTime);

        // Rotación sobre el eje Y
        if (Mathf.Abs(rotation_Input) > 0.01f)
        {
            float rotationAmount = rotation_Input * rotation_Speed * Time.fixedDeltaTime;

            transform.Rotate(0f, rotationAmount, 0f);
        }
    }

    public void TakeScreenShot(InputAction.CallbackContext callbackContext)
    {
        string timestamp = System.DateTime.Now.ToString("MMddyyyy_HHmmss");
        string fileName = $"screenshot_{timestamp}.png";
        ScreenCapture.CaptureScreenshot(fileName);
        SoundManager.Instance.PlaySound("CameraShutter");
    }
}
