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
            resultText.text = didWin ? "WINNER" : "LOSER";
            resultText.color = didWin ? Color.green : Color.red;
        }

        if (teamText != null)
        {
            if (winnerId == 0)
            {
                teamText.text = "BLUE TEAM WINS";
                teamText.color = Color.blue;
            }
            else
            {
                teamText.text = "RED TEAM WINS";
                teamText.color = Color.red;
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