using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
//这个类负责控制实体的统计数据
{
    #region Health
    //角色最大生命值
    public Stat OriginalMaxHealth;
    //角色当前生命值
    public int currentHealth;
    #endregion

    #region AttackDamage
    //角色的基础攻击伤害
    public Stat primaryAttackDamage;
    #endregion

    #region Events
    //记录实体生命值变化这个事件，以便使得只需在血条变动时更新血条，而非一直在更新
    public System.Action onHealthChanged;
    #endregion

    protected virtual void Start()
    {
        //初始时赋予实体其加成过后的最大生命值
        currentHealth = GetFinalMaxHealth();

        //这里的Start函数必须要确保比更新血条UI的Start函数先调用，否则UI会与实际血量不符合
        //若想调整调用顺序，可在Project Settings的Scripts Execution Order处修改
        //Debug.Log("EntityStats Start() Func Called");
    }

    public virtual void GetDamaged(int _damage)
    //有关伤害数值的调用，其他的如击退效果，在继承后重写有需要时调用
    {
        //被攻击时，调用对方的攻击数值，在自己的当前生命值上减掉
        currentHealth -= _damage;
        //被攻击时，调用一下血条UI的更新
        if(onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    public virtual int GetFinalMaxHealth()
    //此函数返回实体的最终最大血量，即等于初始最大血量加上别的加成
    {
        return OriginalMaxHealth.GetValue();
    }

    public virtual void StatsDie()
    //实体死亡函数，主要是用于重写
    {
        //throw new NotImplementedException();
    }
}
