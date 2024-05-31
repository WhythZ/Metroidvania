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

    public override void GetPhysicalDamagedBy(int _damage)
    {
        base.GetPhysicalDamagedBy(_damage);
    }

    public override void StatsDie()
    {
        base.StatsDie();
    }
}
