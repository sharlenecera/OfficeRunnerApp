using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject levelMapCanvas;
    public void Start()
    {
        // Check if the game over state is set
        if (PlayerPrefs.GetInt("GameOver", 0) == 1)
        {
            // Activate the Game Over canvas
            levelMapCanvas.SetActive(true);

            // Reset the game over state
            PlayerPrefs.SetInt("GameOver", 0);
            PlayerPrefs.Save();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Level 1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
