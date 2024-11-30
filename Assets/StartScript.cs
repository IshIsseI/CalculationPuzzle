using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    void Start()
    {
    }
    public void OnStartButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
