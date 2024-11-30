using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void OnStartButton()
    {
        SceneManager.LoadScene("PlusScene");
    }
}
