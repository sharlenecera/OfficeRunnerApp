using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;
    public Button redMenuButton;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        redMenuButton.gameObject.SetActive(false);
        Time.timeScale = 0;
    }
    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        redMenuButton.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
}
