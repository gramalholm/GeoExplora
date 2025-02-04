using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void Home()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
