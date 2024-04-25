using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnemyStats : EntityStats
{
    Enemy enemy;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
    }

    public override void GetPhysicalDamagedBy(int _damage)
    {
        base.GetPhysicalDamagedBy(_damage);
    }
}
