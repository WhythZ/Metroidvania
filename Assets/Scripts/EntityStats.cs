using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EntityStats : MonoBehaviour
//这个类负责控制实体的统计数据
{
    Entity entity;

    #region Health
    [Header("Health Stats")]
    //角色最大生命值
    public Stat originalMaxHealth;
    //角色当前生命值
    public int currentHealth;
    #endregion

    #region PhysicalAttack
    [Header("Physical Attack Stats")]
    //实体的基础攻击伤害
    public Stat primaryAttackDamage;
    //暴击伤害倍率（即乘在最终伤害上的百分比，默认150%，通过赋值函数实现）
    public Stat criticPower;
    //暴击率
    public Stat criticChance;
    #endregion

    #region Attribute
    [Header("Attribute Stats")]
    //力量属性，增加10点攻击力，1%暴击率，2%暴击伤害（这些加成与modifiers的加成不是同一体系的）
    public Stat strength;
    //敏捷属性,增加5点攻击力，2%暴击率，1%闪避率evasion
    public Stat agility;
    //生命力属性，每点增加20点maxHealth，2点物理护甲
    public Stat vitality;
    //智力属性，增加10点法术攻击力，2点法术抵抗力
    public Stat intelligence;
    #endregion

    #region Defence
    [Header("Defence Stats")]
    //闪避率
    public Stat evasionChance;
    //法术抵抗力，提供法术减伤（暂时非百分比）
    public Stat magicalResistance;
    //护甲值，提供物理减伤（暂时非百分比）
    public Stat physicalArmor;
    #endregion

    #region MagicalAttack
    [Header("Magical Attack Stats")]
    //火焰伤害
    public Stat fireAttackDamage;
    //冰冻伤害
    public Stat iceAttackDamage;
    //闪电伤害
    public Stat lightningAttackDamage;
    //处于燃烧状态
    public bool isIgnited;
    //处于冰冻状态
    public bool isChilled;
    //处于眩晕状态
    public bool isShocked;
    #endregion

    #region Events
    //记录实体生命值变化这个事件，以便使得只需在血条变动时更新血条，而非一直在更新
    public System.Action onHealthChanged;
    #endregion

    protected virtual void Start()
    {
        #region SetDefault
        //设置默认最大生命值
        originalMaxHealth.SetDefaultValue(100);
        //设置默认暴击伤害倍率为150%
        criticPower.SetDefaultValue(150);
        //初始暴击率为5%
        criticChance.SetDefaultValue(5);
        //初始闪避率为5%
        evasionChance.SetDefaultValue(5);
        #endregion

        //这里的Start函数必须要确保比更新血条UI的Start函数先调用，否则UI会与实际血量不符合
        //若想调整调用顺序，可在Project Settings的Scripts Execution Order处修改
        //初始时赋予实体其加成过后的最大生命值
        currentHealth = GetFinalMaxHealth();

        //链接实体脚本，会自动检测链接到其子类脚本
        entity = GetComponent<Entity>();
        //Debug.Log(entity.name);
    }

    #region TotalDamage
    public virtual void GetTotalDamageFrom(EntityStats _entityAttackingYou)
    //把物理和法术伤害拆分来一起调用，目的是两者的伤害效果可以一起出现，也更清晰
    {
        this.GetMagicalDamagedBy(_entityAttackingYou.GetFinalMagicalDamage());
        this.GetPhysicalDamagedBy(_entityAttackingYou.GetFinalPhysicalDamage());
    }
    public virtual void GetTotalDamageFrom(EntityStats _entityAttackingYou, Stat _skill)
    //受伤害函数的重载，用于特殊的技能伤害的情况，暂且简单地认为技能伤害为法术伤害，不计算物理伤害
    {
        this.GetMagicalDamagedBy(_entityAttackingYou.GetFinalMagicalDamage() + _skill.GetValue());
    }
    #endregion

    #region MagicalDamaged
    public virtual void GetMagicalDamagedBy(int _damage)
    {
        //如果触发了闪避，则直接返回，不受伤
        if (CanEvade())
        {
            Debug.Log(entity.name + " Evade");
            return;
        }
        else
        {
            //受到的伤害由自身抵抗力减免后作用在生命值上
            currentHealth -= CheckResistance(this, _damage);

            //受攻击的材质变化，使得有针对魔法伤害的闪烁的动画效果
            entity.fx.StartCoroutine("MagicalHitFX");

            //魔法伤害不需要击退，其实是防止有复合伤害时的击退距离更长
            //entity.StartCoroutine("HitKnockback");

            //被攻击时，调用一下血条UI的更新
            if (onHealthChanged != null)
            {
                onHealthChanged();
            }
        }
    }
    public virtual void ApplyAilmentsTo(bool _ignited, bool _chilled, bool _shocked)
    //应用魔法伤害，类似为持续性的debuff
    {
        //如果先前有debuff了，暂时设计为不能再度施加新debuff
        if(isIgnited || isChilled || _shocked)
            return;

        //赋予debuff状态
        isIgnited = _ignited;
        isChilled = _chilled;
        isShocked = _shocked;
    }
    public virtual int CheckResistance(EntityStats _targetStats, int _damage)
    {
        //受到的伤害被法术抵抗力减免
        int _finalDamage = _damage - _targetStats.GetFinalResistance();
        return Mathf.Clamp(_finalDamage, 0, int.MaxValue);
    }
    #endregion

    #region PhysicalDamaged
    public virtual void GetPhysicalDamagedBy(int _damage)
    //有关物理伤害数值的调用，其他的如击退效果，在继承后重写有需要时调用
    {
        //如果触发了闪避，则直接返回，不受伤
        if(CanEvade())
        {
            Debug.Log(entity.name + " Evade");
            return;
        }
        else
        {
            //被攻击时，调用对方的攻击数值，在自己的当前生命值上减掉
            currentHealth -= CheckArmor(this, _damage);

            //受攻击的材质变化，使得有闪烁的动画效果
            entity.fx.StartCoroutine("FlashHitFX");

            //受伤的击退效果
            entity.StartCoroutine("HitKnockback");

            //被攻击时，调用一下血条UI的更新
            if (onHealthChanged != null)
            {
                onHealthChanged();
            }
        }
    }
    public virtual int CheckArmor(EntityStats _targetEntity, int _damage)
    //用于造成伤害前检测一下护甲值对伤害的削弱
    {
        //此处护甲值对伤害减免是非百分比形式的，后续可改为百分比减伤
        int _finalDamage = _damage - _targetEntity.GetFinalArmor();
        //若钳制对象（_totalDamage）小于Clamp内的第二个参数（钳制区间的最小值min），则返回min
        //若大于钳制区间最大值max（此处为int,MaxValue即整形能容纳的最大数），则返回max，若在区间内则返回自身
        //此处返回非负整数，因为伤害不能为负数（变成了治疗）
        return Mathf.Clamp(_finalDamage, 0, int.MaxValue);
    }
    #endregion

    #region Die
    public virtual void StatsDie()
    //实体死亡函数，主要是用于重写
    {
        //这里是状态机转化到deadState的调用
        entity.EntityDie();

        //throw new NotImplementedException();
    }
    #endregion
    
    #region CalculateFinalValues
    public virtual int GetFinalPhysicalDamage()
    {
        //若是触发了暴击，则返回叠加了暴击倍率后的伤害
        if(CanCrit())
        {
            Debug.Log(entity.name + " Crit");

            //注意这里后半部分别用递归，可能多次触发暴击倍率的相乘
            int _nonCritDamage = this.primaryAttackDamage.GetValue() + 10 * strength.GetValue() + 5 * agility.GetValue();
            //使用暴击倍率需要除以100变为浮点数形式，但最终还是要返回一个整型数据
            float _criticPower = GetFinalCriticPower() * 0.01f;

            //从浮点转化为整型
            return Mathf.RoundToInt(_criticPower * _nonCritDamage);
        }
        else
        {
            //返回非暴击的最终的攻击伤害值
            return this.primaryAttackDamage.GetValue() + 10 * strength.GetValue() + 5 * agility.GetValue();
        }
    }
    public virtual int GetFinalMagicalDamage()
    {
        //返回总魔法伤害
        return 10 * intelligence.GetValue() + fireAttackDamage.GetValue() + iceAttackDamage.GetValue() + lightningAttackDamage.GetValue();
    }
    public virtual int GetFinalMaxHealth()
    {
        //此函数返回实体的最终最大血量，即等于初始最大血量加上别的加成
        return originalMaxHealth.GetValue() + 20 * vitality.GetValue();
    }
    public virtual float GetFinalCriticPower()
    {
        //返回最终暴击伤害倍率的%号前部分
        return criticPower.GetValue() + 2 * strength.GetValue();
    }
    public virtual int GetFinalCriticChance()
    {
        //criticChance取值区间为0~100，单位为%
        return Mathf.Clamp(criticChance.GetValue(), 0, 100) + 1 * strength.GetValue() + 2 * agility.GetValue();
    }
    public virtual int GetFinalEvasionChance()
    {
        //criticChance取值区间为0~100，单位为%
        return Mathf.Clamp(evasionChance.GetValue(), 0, 100) + 1 * agility.GetValue();
    }
    public virtual int GetFinalResistance()
    {
        //获取最终法术防御力
        return magicalResistance.GetValue() + 2 * intelligence.GetValue();
    }
    public virtual int GetFinalArmor()
    {
        //返回最终护甲值
        return physicalArmor.GetValue() + 2 * vitality.GetValue();
    }
    #endregion

    #region ChanceAnalyze
    private bool CanCrit()
    //判断是否可以暴击
    {
        //注意Random使用的是Unity内的Random函数
        //通过随机数的方式，判断是否可以暴击
        if(UnityEngine.Random.Range(0,100) <= GetFinalCriticChance())
        {
            return true;
        }
        return false;
    }
    private bool CanEvade()
    {
        //注意Random使用的是Unity内的Random函数
        //通过随机数的方式，判断是否可以闪避
        if (UnityEngine.Random.Range(0, 100) <= GetFinalEvasionChance())
        {
            return true;
        }
        return false;
    }
    #endregion
}
