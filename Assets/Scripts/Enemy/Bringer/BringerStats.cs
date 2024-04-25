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

    public override void GetDamagedBy(int _damage)
    {
        base.GetDamagedBy(_damage);
    }

    public override void StatsDie()
    {
        base.StatsDie();
    }
}
