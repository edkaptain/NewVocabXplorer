using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Text score;
    public Text itemNameText;

    public void UpdateScore(int points)
    {
        score.text = $"Score: {points}/{GameManager.Instance.itemList.Count}";        
    }

    public void UpdateTextItem(string name)
    {
        itemNameText.text = name;
    }
    

}
