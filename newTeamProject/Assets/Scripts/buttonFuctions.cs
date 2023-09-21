using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFuctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }
    public void quit()
    {
        Application.Quit();
    }
    public void respawnPlayer()
    {
        gameManager.instance.stateUnpause();
        gameManager.instance.playerScript.spawnPlayer();
    }
}
