using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI winScoreText;
    [SerializeField]
    private GameObject gameOverCanvas;
    [SerializeField]
    private GameObject winGameOverCanvas;

    private int score = 0;

    public void StopGame(int score)
    {
        // a way to stop the physics of the game, affects time delta time
        // putting it at 0.5f halves the speed
        //      Time.timeScale = 0f;
        // wont use it though since we disabled the player on game over
        this.score = score;
        if (score > 0)
        {
            winGameOverCanvas.SetActive(true);
            winScoreText.text = "Score: " + score.ToString();
        }
        else
        {
            gameOverCanvas.SetActive(true);
        }
    }

    public void Home()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void AddXP()
    {

    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
