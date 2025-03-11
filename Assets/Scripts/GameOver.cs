using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private int score = 0;

    public void StopGame(int score)
    {
        // a way to stop the physics of the game, affects time delta time
        // putting it at 0.5f halves the speed
        //      Time.timeScale = 0f;
        // wont use it though since we disabled the player on game over
        this.score = score;
        scoreText.text = "Score: " + score.ToString();
    }

    public void AddXP()
    {

    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
