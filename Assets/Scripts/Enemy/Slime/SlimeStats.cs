using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStats : EnemyStats
{
    Slime slime;

    protected override void Start()
    {
        base.Start();

        slime = GetComponent<Slime>();
    }
}
