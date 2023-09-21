using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ItemStats : ScriptableObject
{
    public float fireRate;
    public int itemDamage;
    public int shootDistance;
    public int ammoCur;
    public int ammoMax;

    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip shotSound;
}
