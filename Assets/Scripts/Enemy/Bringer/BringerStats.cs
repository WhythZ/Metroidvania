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

    public override void GetPhysicalDamagedBy(int _damage)
    {
        base.GetPhysicalDamagedBy(_damage);
    }

    public override void StatsDie()
    {
        base.StatsDie();
    }
}
