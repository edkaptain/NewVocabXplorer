using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerDescribe : MonoBehaviour
{
    [SerializeField]
    private Text score, textOne;    

    public void UpdateScore(int points)
    {
        score.text = $"Score: {points}/{GameManagerDescribe.Instance.itemList.Count}";
    }

    public void UpdateTextOne(string name)
    {
        textOne.text = name;
    }

    public void CorrectAnswer(string answer)
    {
        textOne.text = $"The correct answer is {answer}";
    }

    private void OnEnable()
    {
        GameManagerDescribe.ClearGame += UpdateScore;
    }

    private void OnDisable()
    {
        GameManagerDescribe.ClearGame += UpdateScore;
    }
}
