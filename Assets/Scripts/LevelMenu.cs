using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;

    private void Awake()
    {
        ButtonsToArray();
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 2);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < unlockedLevel)
            {
                buttons[i].interactable = true;
                // Set the star score to active
                Transform starScore = buttons[i].transform.Find("Star Score");
                if (starScore != null)
                {
                    starScore.gameObject.SetActive(true);
                }
            }
            else
            {
                buttons[i].interactable = false;
            }
        }
    }

    public void OpenLevel(int levelID)
    {
        string levelName = "Level " + levelID;
        SceneManager.LoadScene(levelName);
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
