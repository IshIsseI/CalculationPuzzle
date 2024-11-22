using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public void OnStartButton()
    {
        SceneManager.LoadScene("MainScene");
    }
}
