using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform player, Des;
    public GameObject players;
     void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            players.SetActive(false);
            player.position = Des.position;
            players.SetActive(true);
        }
    }
}
