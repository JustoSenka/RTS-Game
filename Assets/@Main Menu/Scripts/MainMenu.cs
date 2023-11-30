using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameManager", LoadSceneMode.Single);
        SceneManager.LoadScene("Forest", LoadSceneMode.Additive);
        // SceneManager.LoadScene("UI", LoadSceneMode.Additive);
    }
}
