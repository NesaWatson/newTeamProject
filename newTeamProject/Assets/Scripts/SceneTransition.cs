using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //IBoss boss = GetComponent<IBoss>();
           // if (boss != null && boss.IsDefeated)
            //{
                //save player state to gameManager
                gameManager.instance.SavePlayerState();
            //Load the next scene by index

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex + 1);
            //}
        }
    }
}
