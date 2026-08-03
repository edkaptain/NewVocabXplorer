using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public void ChangeScene(int num)
    {
        SceneManager.LoadScene(num);
    }
}