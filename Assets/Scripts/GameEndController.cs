using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndController : MonoBehaviour
{
    // Call this to restart the game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Call this to go to next level or main menu
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
