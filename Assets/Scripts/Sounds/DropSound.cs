using UnityEngine;

public class DropSound : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Item") && AudioManager.Instance.audioSource.isPlaying == false)
        {
            AudioManager.Instance.DropSound();            
        }
    }
}
