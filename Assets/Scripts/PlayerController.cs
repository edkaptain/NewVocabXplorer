using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Controlls gravity, player movement
    //Methods [Jump], [Run]
    public static PlayerController Instance { get; set; }

    // --- Components --- //
    CharacterController controller;
    PlayerInput playerControls;
    InputAction moveAction;

    [Header("VR Center Camera")]
    public Transform headTransform;

    [Header("Movement Speed")]
    public float speed = 3f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private float baseSpeed = 3f;
    public float sprintMultiplier = 2.5f;
    public bool pause = false;

    [Header("Windows Player Input activated")]
    [SerializeField] bool windowsMode;
    [SerializeField, Tooltip("Jump button")] OVRInput.Button jumpButton;

    float yRotation;
    float xRotation;


    public float cameraRotationSensitivity = 1f;
    public UnityEvent events;

    [SerializeField, Tooltip("Enables vibration")] bool vibration = true;


    Vector3 velocity;
    [SerializeField] bool isGrounded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        //moveAction = playerControls.actions.FindAction("Move");
        //Setup Cameras
        //SetUpCameras();
    }
    void Update()
    {
        

        ChecksGravity();
        Vector3 move = new Vector3();

        if (!pause && windowsMode)
        {
            // --- Windows Input --- //
            //Read values of inputs
            Vector2 direction = moveAction.ReadValue<Vector2>();
            move = (transform.right * direction.x) + (transform.forward * direction.y);
            controller.Move(move * speed * Time.deltaTime);
        }
        else
        {
            // --- VR-Player Input --- //          

            // 1.- Reads Joysticks vectors values
            Vector2 leftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector2 rightJoystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            // 2.- Move Player
            if (leftJoystick.magnitude > 0.1f)
            {
                // 3. Chheck the current position of the headset
                Vector3 forwardDirection = headTransform.forward; // Dirección hacia adelante de la cámara.
                forwardDirection.y = 0; // Ignorar inclinación vertical.
                forwardDirection.Normalize();

                // 4. Calcular el vector de movimiento según el joystick y la dirección de la cabeza.
                Vector3 rightDirection = headTransform.right; // Dirección hacia los lados (derecha).
                rightDirection.y = 0;
                rightDirection.Normalize();

                move = (forwardDirection * leftJoystick.y + rightDirection * leftJoystick.x);
            }


            // --- Right joystick camera movement --- //
            RotateCamera(rightJoystick);
            // --- Player Run Movement --- //
            RunVR(OVRInput.Axis1D.PrimaryIndexTrigger);
            // --- Player Jump --- //
            JumpVR();
        }


        // Move player
        controller.Move(move * speed * Time.deltaTime);

        //Gravity controlls
        velocity.y += gravity * Time.deltaTime;
        //Moves the player
        controller.Move(velocity * Time.deltaTime);
    }
    //Change camera's player
    void SetUpCameras()
    {
        //GetComponent<MouseMovement>().enabled = windowsMode;
        transform.Find("Windows Camera").gameObject.SetActive(windowsMode);
        transform.Find("VRCameraRig").transform.Find("ED_OVRCameraRigInteraction").gameObject.SetActive(!windowsMode);
    }

    void ChecksGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
 


    /// <summary>
    /// Jump the player in VR
    /// </summary>
    public void JumpVR()
    {
        if (isGrounded && OVRInput.Get(jumpButton))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    /// <summary>
    /// Rotate the player's camera with joystickstick
    /// </summary>
    void RotateCamera(Vector2 rightJoystick)
    {
        if (rightJoystick.magnitude > 0.1f)
        {
            yRotation += rightJoystick.x * cameraRotationSensitivity;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
    /// <summary>
    /// Runs the player with the button designed
    /// </summary>
    /// <param name="button"></param>
    void RunVR(OVRInput.Axis1D button)
    {
        speed = OVRInput.Get(button) > 0f ? baseSpeed * 1.75f * OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) : baseSpeed;

    }
    

    /// <summary>
    /// Toggles the vibration state (on/off).
    /// </summary>
    public void ToggleVibration()
    {
        vibration = !vibration;
        Debug.Log("Vibration " + (vibration ? "Enabled" : "Disabled"));
    }

    /// <summary>
    /// Controls the user vibration, either for a fixed duration or as an on/off switch.
    /// </summary>
    /// <param name="controller">Joystick to select</param>
    /// <param name="strength">Force to apply (0.0 to 1.0)</param>
    /// <param name="duration">Time in seconds (optional). If not provided, acts as an on/off switch.</param>
    public void VibrateController(OVRInput.Controller controller, float strength, float? duration = null)
    {
        if (!vibration) return;

        if (duration.HasValue)
        {
            // Vibrate for a fixed duration
            StartCoroutine(VibrateForDuration(controller, strength, duration.Value));
        }
        else
        {
            // Vibrate indefinitely (until stopped manually)
            OVRInput.SetControllerVibration(1f, strength, controller);
        }
    }

    /// <summary>
    /// Stops the vibration of the selected controller.
    /// </summary>
    /// <param name="controller">Joystick to stop vibration</param>
    public void StopVibration(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(0f, 0f, controller);
    }

    /// <summary>
    /// Coroutine that stops vibration after a certain duration.
    /// </summary>
    /// <param name="controller">Joystick to select</param>
    /// <param name="strength">Force to apply</param>
    /// <param name="duration">Time in seconds</param>
    private IEnumerator VibrateForDuration(OVRInput.Controller controller, float strength, float duration)
    {
        OVRInput.SetControllerVibration(1f, strength, controller);
        yield return new WaitForSeconds(duration);
        StopVibration(controller);
    }


}



