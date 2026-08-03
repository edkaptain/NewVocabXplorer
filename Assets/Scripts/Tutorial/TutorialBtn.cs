using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{    

    public void ScaleButton(float scale)
    {
        transform.localScale = new Vector3(scale,scale);
    }
}
