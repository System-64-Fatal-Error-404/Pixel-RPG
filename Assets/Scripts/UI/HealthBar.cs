using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    PlayerMovement plr;
    private float percentage;

    private float plrHp;

    private void Start()
    {
        plr = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        plrHp = plr._hp;
    }

    private void Update()
    {
        healthBar.value = plr._hp/plrHp;
    }
}
