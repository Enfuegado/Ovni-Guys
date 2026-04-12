using UnityEngine;
using TMPro;

public class GameEndUIController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI teamText;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

public void ShowResult(bool didWin, int winnerId)
{
    if (panel != null)
        panel.SetActive(true);

    if (resultText != null)
    {
        resultText.text  = didWin ? "WINNER" : "LOSER";
        resultText.color = didWin
            ? new Color(0x30 / 255f, 0x90 / 255f, 0xe0 / 255f)  
            : new Color(0xd8 / 255f, 0x30 / 255f, 0x30 / 255f);  
    }

    if (teamText != null)
    {
        if (winnerId == 0)
        {
            teamText.text  = "BLUE TEAM WINS";
            teamText.color = new Color(0x30 / 255f, 0x90 / 255f, 0xe0 / 255f);
        }
        else
        {
            teamText.text  = "RED TEAM WINS";
            teamText.color = new Color(0xd8 / 255f, 0x30 / 255f, 0x30 / 255f);
        }
    }

    Time.timeScale = 0f;
}

    public void OnExitButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}