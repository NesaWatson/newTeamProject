using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shuriken : MonoBehaviour
{
    [SerializeField] Rigidbody rb; 

    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] int destroyTime;

    private GameObject shooter;

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }
    void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        Destroy(gameObject, destroyTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }

        Destroy(gameObject);
    }

}
