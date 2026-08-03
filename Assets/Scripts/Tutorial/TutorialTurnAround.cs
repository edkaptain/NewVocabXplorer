using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class TutorialTurnAround : MonoBehaviour
{
    public List<GameObject> vectors = new List<GameObject>();
    public int points;
    public GameObject btn;
    public GameObject player;

    private void OnValidate()
    {
        CollectVectors();
    }
    [ContextMenu("Collect vectors")]
    private void CollectVectors()
    {
        vectors.Clear();

        foreach (Transform child in transform)
        {
            vectors.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        vectors[0].gameObject.SetActive(true);
        vectors[0].GetComponent<TurnAroundItem>().TurnText(player);
    }
    [ContextMenu("Add points")]
    public void AddPoints()
    {
        points++;

        if(points >= vectors.Count)
        {
            btn.SetActive(true);
            AudioManager.Instance.Sucess();
            Debug.LogWarning("This part was completed");
            return;
        }

        vectors[points].SetActive(true);
        vectors[points].GetComponent<TurnAroundItem>().TurnText(player);

    }
}
