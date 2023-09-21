using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{
    [SerializeField] ItemStats item;
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.name);
            gameManager.instance.playerScript.itemPickup(item);
            Destroy(gameObject);
        }
    }
}
