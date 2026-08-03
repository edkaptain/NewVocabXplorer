using UnityEngine;

public class TurnAroundItem : MonoBehaviour
{
    public TutorialTurnAround turnAround;

    [Header("Materials")]
    public Material originalMaterial;
    public Material greenMaterial;

    public GameObject sphere;
    public GameObject txt;

   public Camera playerCamera;

    public float lookTimer;
    private bool completed;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

    }

    private void Update()
    {
        if (completed || playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {

            UpdateColor(true);
            // Comprueba si el objeto golpeado es este objeto
            TurnAroundItem observedItem =
                hit.collider.GetComponentInParent<TurnAroundItem>();

            if (observedItem == this)
            {
                lookTimer += Time.deltaTime;

                if (lookTimer >= 1.25f)
                {
                    completed = true;
                    AudioManager.Instance.SystemNotification(true);

                    transform.gameObject.SetActive(false);

                    turnAround.AddPoints();
                }

                return;
            }
            else
            {
                UpdateColor(false);
                lookTimer = 0f;
            }
        }

        // Se reinicia si deja de mirar el objeto
        lookTimer = 0f;
    }

    public void UpdateColor(bool status)
    {
        
        sphere.GetComponent<MeshRenderer>().material =
            status ? greenMaterial : originalMaterial;
    }

    public void TurnText(GameObject obj)
    {
        txt.transform.LookAt(obj.transform);
        txt.transform.Rotate(0, 180f, 0);
    }
}