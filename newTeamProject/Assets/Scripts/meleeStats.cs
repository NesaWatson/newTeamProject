using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class meleeStats : ScriptableObject
{
    public GameObject weaponModel;

    public AudioClip attackSound;
    public AudioClip hitSound;

    public ParticleSystem hitEffect;

    public float attackSpeed;
    public float weaponRange;
    public float weaponDamage;



}
