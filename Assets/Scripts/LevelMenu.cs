using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;
    [SerializeField]
    private int level1Score;
    [SerializeField]
    private int level2Score;
    [SerializeField]
    private int level3Score;
    [SerializeField]
    private int startingUnlockedLevel = 1;

    private void Awake()
    {
        ButtonsToArray();
        GetStarScores();
        UpdateStarScores();
    }

    public void OpenLevel(int levelID)
    {
        string levelName = "Level " + levelID;
        SceneManager.LoadScene(levelName);
    }

    void GetStarScores() // for testing purposes
    {
        level1Score = PlayerPrefs.GetInt("Level1Stars", 0);
        level2Score = PlayerPrefs.GetInt("Level2Stars", 0);
        level3Score = PlayerPrefs.GetInt("Level3Stars", 0);
    }

    void UpdateStarScores()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", startingUnlockedLevel);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < unlockedLevel)
            {
                buttons[i].interactable = true;
                // Set the star score to active
                Transform starScore = buttons[i].transform.Find("Star Scores");
                if (starScore != null)
                {
                    if (i != unlockedLevel)
                    {
                        if (PlayerPrefs.HasKey("Level" + i + "Stars"))
                        {
                            int stars = PlayerPrefs.GetInt("Level" + i + "Stars");
                            for (int j = 0; j < stars; j++)
                            {
                                starScore.GetChild(j).gameObject.SetActive(true);
                            }
                        }
                    }
                    starScore.gameObject.SetActive(true);
                }
            }
            else
            {
                buttons[i].interactable = false;
            }
        }
    }

    void ButtonsToArray()
    {
        int childCount = levelButtons.transform.childCount;
        buttons = new Button[childCount];
        for (int i = 0; i < childCount; i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }

    public void ToggleLevelInfo(GameObject targetObject)
    {
        // Toggle a specific child of all other level objects off
        foreach (Transform child in levelButtons.transform)
        {
            // Assuming first child is the info object
            Transform infoObjects = child.GetChild(0);
            Debug.Log(targetObject.transform.parent.gameObject.name);
            if (infoObjects != null)
            {
                Debug.Log("not null");
                if (child.gameObject.name != targetObject.transform.parent.gameObject.name)
                {
                    infoObjects.gameObject.SetActive(false);
                    Debug.Log("irrelevant set to false");
                }
                else
                {
                    targetObject.SetActive(true);
                    // Debug.Log(child.gameObject.name);
                    Debug.Log("set to true");
                }
            }
        }
        //Toggle the active state of the target object
        // targetObject.SetActive(!targetObject.activeSelf);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }
}
