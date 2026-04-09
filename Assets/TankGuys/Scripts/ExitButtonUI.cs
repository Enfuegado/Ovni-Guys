using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButtonUI : MonoBehaviour
{
    public void OnExitButton()
    {
        Debug.Log("BOTON PRESIONADO");

        Matchmaking mm = FindObjectOfType<Matchmaking>();

        if (mm != null)
        {
            mm.Cleanup();
            Destroy(mm.gameObject);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}