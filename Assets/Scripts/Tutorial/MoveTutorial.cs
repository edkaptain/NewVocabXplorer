using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class MoveTutorial : MonoBehaviour
{
    public List<GameObject> vectors = new List<GameObject>();
    public int points;
    public GameObject btn;
    public GameObject player;

   
    private void OnValidate()
    {
        CollectVectors();
    }

    private void CollectVectors()
    {
        vectors.Clear();

        foreach (Transform child in transform) { 
            vectors.Add(child.gameObject);
            SphereCollider sphere = child.GetComponent<SphereCollider>();

            if (sphere != null) { 
                sphere.isTrigger = true;    
            }

            if(child.GetComponent<DetectUser>() == null)
            {
                child.gameObject.AddComponent<DetectUser>();
                child.gameObject.GetComponent<DetectUser>().moveTutorial = this;
            }
            child.gameObject.SetActive(false);
        }
        vectors[0].gameObject.SetActive(true);
    }

    
    public void AddPoints()
    {
        points++;

        if (points >= vectors.Count)
        {
            btn.SetActive(true);
            AudioManager.Instance.Sucess();
            return;
        }

        vectors[points].SetActive(true);
        vectors[points].transform.GetChild(0).LookAt(player.transform);
        vectors[points].transform.GetChild(0).Rotate(0f, 180f, 0f);
    }


}
