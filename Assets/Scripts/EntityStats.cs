using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
//这个类负责控制实体的统计数据
{
    #region Health
    //角色最大生命值
    public Stat maxHealth;
    //角色当前生命值
    public int currentHealth;
    #endregion

    #region AttackDamage
    //角色的基础攻击伤害
    public Stat primaryAttackDamage;
    #endregion

    protected virtual void Start()
    {
        //初始时赋予实体其最大生命值
        currentHealth = maxHealth.GetValue();
    }

    public virtual void GetDamaged(int _damage)
    {
        //被攻击时，调用对方的攻击数值，在自己的当前生命值上减掉
        currentHealth -= _damage;
    }
}
