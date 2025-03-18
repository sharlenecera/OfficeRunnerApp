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

    public void SetLevel1StarScore(int score)
    {
        // Level 1 score multiplier = 237
        int threeStars = 230;
        int twoStars = 200;
        int oneStar = 150;

        PlayerPrefs.SetInt("Level1Score", score);
        if (score >= threeStars)
        {
            PlayerPrefs.SetInt("Level1Stars", 3);
        }
        else if (score >= twoStars)
        {
            PlayerPrefs.SetInt("Level1Stars", 2);
        }
        else if (score >= oneStar)
        {
            PlayerPrefs.SetInt("Level1Stars", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Level1Stars", 0);
        }
    }
    public void SetLevel2StarScore(int score)
    {
        // Level 2 score multiplier = 268
        int threeStars = 260;
        int twoStars = 150;
        int oneStar = 60;

        PlayerPrefs.SetInt("Level2Score", score);
        if (score >= threeStars)
        {
            PlayerPrefs.SetInt("Level2Stars", 3);
        }
        else if (score >= twoStars)
        {
            PlayerPrefs.SetInt("Level2Stars", 2);
        }
        else if (score >= oneStar)
        {
            PlayerPrefs.SetInt("Level2Stars", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Level2Stars", 0);
        }
    }
    public void SetLevel3StarScore(int score)
    {
        // Level 3 score multiplier = 294
        int threeStars = 290;
        int twoStars = 260;
        int oneStar = 210;

        PlayerPrefs.SetInt("Level3Score", score);
        if (score >= threeStars)
        {
            PlayerPrefs.SetInt("Level3Stars", 3);
        }
        else if (score >= twoStars)
        {
            PlayerPrefs.SetInt("Level3Stars", 2);
        }
        else if (score >= oneStar)
        {
            PlayerPrefs.SetInt("Level3Stars", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Level3Stars", 0);
        }
    }

    public void AddXP()
    {

    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
