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

    #region Attack
    [Header("Attack Stats")]
    //暴击伤害倍率（百分比，大于100）
    public Stat criticPower;
    //暴击率（百分比）
    public Stat criticChance;
    
    //实体的基础物理攻击伤害
    public Stat primaryPhysicalDamage;
    //火焰伤害
    public Stat fireAttackDamage;
    //冰冻伤害
    public Stat iceAttackDamage;
    //闪电伤害
    public Stat lightningAttackDamage;
    #endregion

    #region Ailments
    //处于燃烧状态
    public bool isIgnited;
    //处于冰冻状态
    public bool isChilled;
    //处于眩晕状态
    public bool isShocked;
    #endregion

    #region Defence
    [Header("Defence Stats")]
    //闪避率（百分比）
    public Stat evasionChance;
    //护甲值，提供物理减伤（百分比）
    public Stat physicalArmor;
    //法术抵抗力，提供法术减伤（百分比）
    public Stat magicalResistance;
    #endregion

    #region Events
    //记录实体生命值变化这个事件，以便使得只需在血条变动时更新血条，而非一直在更新
    public System.Action onHealthChanged;
    #endregion

    protected virtual void Start()
    {
        //这里的Start函数必须要确保比更新血条UI的Start函数先调用，否则UI会与实际血量不符合
        //若想调整调用顺序，可在Project Settings的Scripts Execution Order处修改
        //Debug.Log("EntityStats Start() Func Called");
        //初始时赋予实体其加成过后的最大生命值
        currentHealth = GetFinalMaxHealth();

        //链接实体脚本，会自动检测链接到其子类脚本
        entity = GetComponent<Entity>();
        //Debug.Log(entity.name);

        //初始时清空所有负面buff
        GetAilments(false, false, false);
    }

    #region TotalDamage
    public virtual void GetTotalNormalDmgFrom(EntityStats _attackingEntity, bool _doPhysic, bool _doMagic)
    //提供一种调用全部伤害的函数，所有触发伤害都调用此函数，不建议单独调用
    //第二、第三参数位要传入是否只触发单独一类伤害或者两者一起触发的布尔值
    {
        //若是对方基础伤害为0，则不应进行最终值的获取（其中还要进行暴击判定，判定若是成功了还会返回暴击效果，这是不需要的，因为暴击与否都是0伤害）
        if (_attackingEntity.GetNonCritMagicalDamage() > 0 && _doMagic)
        {
            //数值伤害的施加
            this.GetMagicalDamagedBy(_attackingEntity.GetFinalMagicalDamage());

            //负面效果的施加
            CheckAilmentsFrom(_attackingEntity); 
        }
        if(_attackingEntity.GetNonCritPhysicalDamage() > 0 && _doPhysic)
        {
            this.GetPhysicalDamagedBy(_attackingEntity.GetFinalPhysicalDamage());
        }
    }
    public virtual void GetTotalSkillDmgFrom(EntityStats _attackingEntity, int _skillDmg, bool _isMagical)
    //技能的伤害（物理、魔法）的伤害的，传入造成伤害者和造成的伤害大小、伤害的类型（是魔法的话还要进行debuff施加判定）
    //技能伤害与对方原有伤害类型数值（非暴击）进行叠加，吃技能伤害时不会发生暴击
    {
        if(_isMagical)
        {
            this.GetMagicalDamagedBy(_attackingEntity.GetNonCritMagicalDamage() + _skillDmg);

            //debuff施加
            CheckAilmentsFrom(_attackingEntity);
        }
        if(!_isMagical)
        {
            this.GetPhysicalDamagedBy(_attackingEntity.GetFinalPhysicalDamage() + _skillDmg); 
        }
    }
    #endregion

    #region Ailments
    public virtual void CheckAilmentsFrom(EntityStats _entity)
    {
        //存储攻击自己的实体的魔法伤害数据
        int _fireDmg = _entity.fireAttackDamage.GetValue();
        int _iceDmg = _entity.iceAttackDamage.GetValue();
        int _lightDmg = _entity.lightningAttackDamage.GetValue();
        //选出最大的数据，其施加其对应的debuff，因为只能施加一种debuff，故选取伤害最大的
        bool _canApplyIgnite = _fireDmg > _iceDmg && _fireDmg > _lightDmg;
        bool _canApplyChill = _iceDmg > _fireDmg && _iceDmg > _lightDmg;
        bool _canApplyShock = _lightDmg > _fireDmg && _lightDmg > _iceDmg;
        //施加负面效果
        GetAilments(_canApplyIgnite, _canApplyChill, _canApplyShock);
    }
    public virtual void GetAilments(bool _ignited, bool _chilled, bool _shocked)
    //应用魔法伤害，类似为持续性的debuff
    {
        //如果先前有debuff了，暂时设计为不能再度施加新debuff
        if (isIgnited || isChilled || _shocked)
            return;

        //赋予debuff状态
        isIgnited = _ignited;
        isChilled = _chilled;
        isShocked = _shocked;
    }
    #endregion

    #region MagicalDamaged
    public virtual void GetMagicalDamagedBy(int _damage)
    //这里是数值的伤害的施加，而debuff的判定（需要传入敌方Stats）不放在这里，因为有些纯魔法攻击不会产生debuff
    {
        //如果触发了闪避，则直接返回，不受伤
        if (CanEvade())
        {
            return;
        }
        else
        {
            #region AttackedFX
            //受攻击的材质变化，使得有针对魔法伤害的闪烁的动画效果
            entity.fx.StartCoroutine("MagicalHitFX");

            //弹出伤害数值文本效果，玩家不弹
            if (entity.GetComponent<Player>() == null)
                entity.fx.CreatPopUpText(_damage.ToString(), Color.white);

            //魔法伤害不需要击退，其实是防止有复合伤害时的击退距离更长
            //entity.StartCoroutine("HitKnockback");
            #endregion

            //受到的伤害由自身抵抗力减免后作用在生命值上
            currentHealth -= CheckResistance(this, _damage);

            //被攻击时，调用一下血条UI的更新
            if (onHealthChanged != null)
            {
                onHealthChanged();
            }
        }
    }
    public virtual int CheckResistance(EntityStats _targetStats, int _damage)
    {
        //记录自己的法术抵抗力（0~70）间，即伤害减免不能超过70%，即最终最少也要收到对方伤害的30%
        int _resistance = _targetStats.GetFinalResistance();
        _resistance = Mathf.Clamp(_resistance, 0, 70);
        //计算减伤率（百分比）
        float _resistancePercentage = _resistance * 0.01f;
        //计算最终承受伤害（浮点数）
        float _checkedFinalDamage = _damage * (1 - _resistancePercentage);
        //转化为整型伤害
        return Mathf.RoundToInt(_checkedFinalDamage);
    }
    #endregion

    #region PhysicalDamaged
    public virtual void GetPhysicalDamagedBy(int _damage)
    //有关物理伤害数值的调用，其他的如击退效果，在继承后重写有需要时调用
    {
        //如果触发了闪避，则直接返回，不受伤
        if(CanEvade())
        {
            return;
        }
        else
        {
            #region AttackedFX
            //受攻击的材质变化，使得有闪烁的动画效果
            entity.fx.StartCoroutine("FlashHitFX");

            //弹出伤害数值文本效果，玩家不弹
            if (entity.GetComponent<Player>() == null)
                entity.fx.CreatPopUpText(_damage.ToString(), Color.white);

            //受伤的击退效果
            entity.StartCoroutine("HitKnockback");
            #endregion

            //被攻击时，调用对方的攻击数值，在自己的当前生命值上减掉
            currentHealth -= CheckArmor(this, _damage);

            //被攻击时，调用一下血条UI的更新
            if (onHealthChanged != null)
            {
                onHealthChanged();
            }
        }
    }
    public virtual int CheckArmor(EntityStats _targetStats, int _damage)
    //用于造成伤害前检测一下护甲值对伤害的削弱
    {
        //记录自己的物理护甲（0~70）间，即伤害减免不能超过70%，即最终最少也要收到对方伤害的30%
        int _armor = _targetStats.GetFinalArmor();
        //若钳制对象（_armor）小于第二个参数（钳制区间的最小值min），则返回min；若大于钳制区间最大值max则返回max;若在区间内则返回自身
        _armor = Mathf.Clamp(_armor, 0, 70);
        //计算减伤率（百分比）
        float _armorPercentage = _armor * 0.01f;
        //计算最终承受伤害（浮点数）
        float _checkedFinalDamage = _damage * (1 - _armorPercentage);
        //转化为整型伤害
        return Mathf.RoundToInt(_checkedFinalDamage);
    }
    #endregion

    #region FinalValues
    public virtual int GetFinalPhysicalDamage()
    {
        //记录下来非暴击伤害
        int _nonCritDamage = GetNonCritPhysicalDamage();

        //若是触发了暴击，则返回叠加了暴击倍率后的伤害
        if (CanCrit())
        {
            //使用暴击倍率需要除以100变为浮点数形式，但最终还是要返回一个整型数据
            float _criticPower = GetFinalCriticPower() * 0.01f;

            //从浮点转化为整型
            return Mathf.RoundToInt(_criticPower * _nonCritDamage);
        }
        else
        {
            //返回非暴击的最终的攻击伤害值
            return _nonCritDamage;
        }
    }
    public virtual int GetNonCritPhysicalDamage()
    //得到不进行暴击判定的原始伤害，用于给UI赋值，不然UI那边刷新数据时还可能触发暴击，导致面板伤害偏高
    {
        return primaryPhysicalDamage.GetValue() + 10 * strength.GetValue() + 5 * agility.GetValue();
    }
    public virtual int GetFinalMagicalDamage()
    {
        //记录非暴击总魔法伤害
        int _nonCritDamage = GetNonCritMagicalDamage();

        if (CanCrit())
        {
            float _criticPower = GetFinalCriticPower() * 0.01f;
            return Mathf.RoundToInt(_criticPower * _nonCritDamage);
        }
        else
        {
            return _nonCritDamage;
        }
    }
    public virtual int GetNonCritMagicalDamage()
    {
        return 10 * intelligence.GetValue() + fireAttackDamage.GetValue() + iceAttackDamage.GetValue() + lightningAttackDamage.GetValue();
    }
    public virtual int GetFinalMaxHealth()
    {
        //此函数返回实体的最终最大血量，即等于初始最大血量加上别的加成
        return originalMaxHealth.GetValue() + 20 * vitality.GetValue();
    }
    public virtual int GetFinalCriticPower()
    {
        //暴击伤害倍率
        int _finalCriticPower = criticPower.GetValue() + 2 * strength.GetValue();
        //保证倍率要大于100%，上限为int.MaxValue即整型边界
        _finalCriticPower = Mathf.Clamp(_finalCriticPower, 100, int.MaxValue);
        //返回最终暴击伤害倍率的%号前部分
        return _finalCriticPower;
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

    #region StatsDie
    public virtual void StatsDie()
    //实体死亡函数，主要是用于重写
    {
        //这里是状态机转化到deadState的调用
        entity.EntityDie();
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
            //文字弹出效果，玩家和怪物都弹
            entity.fx.CreatPopUpText("Miss", Color.yellow);

            return true;
        }
        return false;
    }
    #endregion

    #region StatTypeMapping
    public int GetValueOfStatType(StatType _statType)
    //返回对应的数值
    {
        if (_statType == StatType.health) { return GetFinalMaxHealth(); }
        if (_statType == StatType.strength) { return strength.GetValue(); }
        if (_statType == StatType.agility) { return agility.GetValue(); }
        if (_statType == StatType.vitality) { return vitality.GetValue(); }
        if (_statType == StatType.intelligence) { return intelligence.GetValue(); }
        if (_statType == StatType.physicalDamage) { return GetNonCritPhysicalDamage(); }
        if (_statType == StatType.critChance) { return GetFinalCriticChance(); }
        if (_statType == StatType.critPower) { return GetFinalCriticPower(); }
        if (_statType == StatType.magicalDamage) { return GetNonCritMagicalDamage(); }
        if (_statType == StatType.evasion) { return GetFinalEvasionChance(); }
        if (_statType == StatType.armor) { return GetFinalArmor(); }
        if (_statType == StatType.resistance) { return GetFinalResistance(); }

        //注意这里返回空，不然会报错：并非所有的代码路径都返回值
        return 0;
    }
    #endregion
}

#region StatTypeEnum
public enum StatType
//在Hierarchy给对象上的脚本内的变量赋值的时候，若变量 数据类型是StatType，则可以直接选取此结构内的各成员作为值赋予变量
{
    health,
    strength,
    agility,
    vitality,
    intelligence,
    physicalDamage,
    critChance,
    critPower,
    magicalDamage,
    evasion,
    armor,
    resistance
}
#endregion