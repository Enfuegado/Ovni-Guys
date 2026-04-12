using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int blueScore = 0;
    private int redScore = 0;

    void Start()
    {
        UpdateText();
    }

    public void SetBlueScore(int value)
    {
        blueScore = value;
        UpdateText();
    }

    public void SetRedScore(int value)
    {
        redScore = value;
        UpdateText();
    }

    void UpdateText()
    {
        scoreText.text =
            $"<color=#3090e0>BLUE {blueScore}</color> <color=white>|</color> <color=#d83030>{redScore} RED</color>";
    }
}