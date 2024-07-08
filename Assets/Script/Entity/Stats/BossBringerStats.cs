using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerStats : EnemyStats
{
    Bringer bossBringer;

    protected override void Start()
    {
        base.Start();

        bossBringer = GetComponent<Bringer>();
    }
}
