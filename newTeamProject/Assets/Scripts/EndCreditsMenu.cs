using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCreditsMenu : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
