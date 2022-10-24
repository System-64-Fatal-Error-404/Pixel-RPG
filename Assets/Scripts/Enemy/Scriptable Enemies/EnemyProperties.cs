using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyProperties : ScriptableObject
{
    public string enemyName;
    public float enemyHP;
    public float enemyAP;
    public float enemySpeed;
    public float enemyMaximumSpeed;

    public float pov;
    public float range;
    
    public AudioClip attackSound;

    public bool canFly;
}
