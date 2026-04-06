using UnityEngine;
using TMPro;

public class GameStartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownTime = 3f;

    private float startTime;
    private float endTime;
    private bool started = false;

    void Start()
    {
        startTime = Time.realtimeSinceStartup;
        endTime = startTime + countdownTime;
    }

    void Update()
    {
        float remaining = endTime - Time.realtimeSinceStartup;

        if (remaining > 0f)
        {
            countdownText.text = Mathf.Ceil(remaining).ToString();
        }
        else if (!started)
        {
            started = true;

            countdownText.text = "GO";
            EnablePlayer();
            Invoke(nameof(Hide), 0.5f);
        }
    }

    void EnablePlayer()
    {
        GameManagerHTTP gm = FindObjectOfType<GameManagerHTTP>();

        if (gm == null) return;

        GameObject player = gm.GetLocalPlayer();

        if (player == null) return;

        var controller = player.GetComponent<PlayerLocalController>();

        if (controller != null)
            controller.enabled = true;
    }

    void Hide()
    {
        countdownText.gameObject.SetActive(false);
    }
}