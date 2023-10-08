using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class meleeStats : ScriptableObject
{
    public GameObject weaponModel;
    public string weaponName;

    public AudioClip attackSound;
    public AudioClip hitSound;

    public ParticleSystem hitEffect;

    public float attackSpeed;
    public float weaponRange;
    public int weaponDamage;

    public bool doesBleed;
    public float bleedTime = 5f;
    public float bleedDPS = 0.5f;

    public bool canBurn;
    public float burnTime = 5f;
    public float burnDPS = 1f;



}
