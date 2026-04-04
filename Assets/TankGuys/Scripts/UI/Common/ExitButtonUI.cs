using UnityEngine;

public class ExitButtonUI : MonoBehaviour
{
    public void OnExitButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}