using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;

    public void ColliderEnabled()
    {
        weaponCollider.enabled = true;
    }

    public void ColliderDisabled()
    {
        weaponCollider.enabled = false;
    }


}
