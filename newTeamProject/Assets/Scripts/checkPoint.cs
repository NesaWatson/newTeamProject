using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")&& gameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            
            gameManager.instance.playerSpawnPos.transform.position= transform.position;
            playerController player = other.GetComponent<playerController>();
            if(player != null)
            {
                player.PlayerCheckpointRefresh();
            }
            
            StartCoroutine(gameManager.instance.checkPointSpot());
        }
       
    }
}
