using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerStats : EnemyStats
{
    Bringer bringer;

    protected override void Start()
    {
        base.Start();

        bringer = GetComponent<Bringer>();
    }
}
