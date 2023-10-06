using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeItemPickup : MonoBehaviour
{
    [SerializeField] meleeStats meleeItem;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager.instance.playerScript.PickupMeleeWeapon(meleeItem);
        Destroy(gameObject);
    }
}
