using UnityEngine;

public class SunControl : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.5f;
 
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime * speed, Time.deltaTime * speed, 0f);
    }
}
