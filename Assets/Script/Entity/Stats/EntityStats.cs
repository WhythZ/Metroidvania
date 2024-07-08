using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

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
    fireDamage,
    iceDamage,
    lightningDamage,
    evasion,
    armor,
    resistance,
    fireballDamage,
    iceballDamage
}
#endregion

public class EntityStats : MonoBehaviour
//这个类负责控制实体的统计数据
{
    #region Components
    private Entity entity;
    private EntityBuffs buf;
    private EntityFX fx;
    #endregion

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
    //暴击率（百分比）
    public Stat criticChance;
    //暴击伤害倍率（百分比，大于100）
    public Stat criticPower;
    
    //实体的基础物理攻击伤害
    public Stat primaryPhysicalDamage;
    //火焰伤害
    public Stat fireAttackDamage;
    //冰冻伤害
    public Stat iceAttackDamage;
    //闪电伤害
    public Stat lightningAttackDamage;
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

    #region Default
    //存储原有值
    private int defaultIntEvasion;
    private int defaultIntArmor;
    private int defaultIntResistance;
    #endregion

    #region Events
    //记录实体生命值变化这个事件，以便使得只需在血条变动时更新血条，而非一直在更新
    public System.Action onHealthChanged;
    #endregion

    protected virtual void Start()
    {
        #region Components
        //链接实体脚本，会自动检测链接到其子类脚本
        entity = GetComponent<Entity>();
        //Debug.Log(entity.name);
        //链接到Buffs脚本
        buf = GetComponent<EntityBuffs>();
        //链接到效果脚本
        fx = GetComponent<EntityFX>();
        #endregion

        //这里的Start函数必须要确保比更新血条UI的Start函数先调用，否则UI会与实际血量不符合；若想调整调用顺序，可在Project Settings的Scripts Execution Order处修改
        //初始时赋予实体其加成过后的最大生命值
        currentHealth = GetFinalMaxHealth();
        //Debug.Log("EntityStats Start() Func Called");
    }

    #region GetDamaged
    //整体考虑实体受到的攻击，复合了实体受到的物理和魔法两类伤害
    #region TotalDamage
    public virtual void GetTotalNormalDmgFrom(EntityStats _attackingEntity, bool _doPhysic, bool _doMagic)
    //第二、第三参数位要传入是否只触发单独一类伤害或者两者一起触发的布尔值
    {
        #region Evade&Crit
        //记录对方伤害、暴击等属性
        int _attackingCritPower = _attackingEntity.GetFinalCriticPower();
        int _physicDmg = _attackingEntity.GetNonCritPhysicalDamage();
        int _magicDmg = _attackingEntity.GetNonCritMagicalDamage();

        //如果触发了闪避，则直接返回，不受伤
        if (CanEvade())
        {
            return;
        }

        //如果对方触发了暴击，则读取对方暴击伤害进行受伤的增伤
        if (CanCrit(_attackingEntity))
        {
            //使用暴击倍率需要除以100变为浮点数形式，但最终还是要返回一个整型数据
            float _criticPowerPercentage = GetFinalCriticPower() * 0.01f;
            //从浮点转化为整型
            _physicDmg = Mathf.RoundToInt(_criticPowerPercentage * _physicDmg);
            _magicDmg = Mathf.RoundToInt(_criticPowerPercentage * _magicDmg);
        }
        #endregion

        #region AttackedFX
        //受攻击的音效
        AudioManager.instance.PlaySFX(12, null);
        //受攻击的粒子效果，在自己（受攻击者）身上
        fx.CreateHitFX00(this.transform);
        #endregion

        //若是对方基础伤害为0，则不应进行伤害
        if (_attackingEntity.GetNonCritPhysicalDamage() > 0 && _doPhysic)
        {
            //物理数值伤害的施加
            this.GetPhysicalDamagedBy(_physicDmg);
        }
        if (_attackingEntity.GetNonCritMagicalDamage() > 0 && _doMagic)
        {
            //魔法数值伤害的施加
            this.GetMagicalDamagedBy(_magicDmg);

            //魔法元素相关Buff的施加
            buf.CheckBuffsFrom(_attackingEntity); 
        }
    }
    public virtual void GetTotalSkillDmgFrom(EntityStats _attackingEntity, int _skillDmg, bool _doPhysic, bool _doMagic, bool _ignite, bool _chill, bool _shock)
    //用于如技能伤害的触发，传入攻击者和造成的伤害大小、伤害的类型（有魔法的话还要传入Debuff施加判定）
    {
        #region Evade&Crit
        //记录对方伤害、暴击等属性
        int _attackingCritPower = _attackingEntity.GetFinalCriticPower();
        int _physicDmg = _attackingEntity.GetNonCritPhysicalDamage();
        int _magicDmg = _attackingEntity.GetNonCritMagicalDamage();

        //如果触发了闪避，则直接返回，不受伤
        if (CanEvade())
        {
            return;
        }

        //如果对方触发了暴击，则读取对方暴击伤害进行受伤的增伤
        if (CanCrit(_attackingEntity))
        {
            //使用暴击倍率需要除以100变为浮点数形式，但最终还是要返回一个整型数据
            float _criticPowerPercentage = GetFinalCriticPower() * 0.01f;
            //从浮点转化为整型
            _physicDmg = Mathf.RoundToInt(_criticPowerPercentage * _physicDmg);
            _magicDmg = Mathf.RoundToInt(_criticPowerPercentage * _magicDmg);
        }
        #endregion

        #region AttackedFX
        //受攻击的音效
        AudioManager.instance.PlaySFX(12, null);
        //受攻击的粒子效果，在自己（受攻击者）身上
        fx.CreateHitFX00(this.transform);
        #endregion

        //技能一定有伤害，所以不用检查基础伤害是否大于零
        if (_doPhysic)
        {
            //物理数值伤害的施加
            this.GetPhysicalDamagedBy(_physicDmg + _skillDmg); 
        }
        if (_doMagic)
        {
            //魔法数值伤害的施加
            this.GetMagicalDamagedBy(_magicDmg + _skillDmg);

            //debuff施加
            buf.ApplyBuffs(_ignite, _chill, _shock);
        }
    }
    #endregion

    //单独考虑实体受到的物理伤害值
    #region PhysicalDamaged
    public virtual void GetPhysicalDamagedBy(int _damage)
    //有关物理伤害数值的调用，其他的如击退效果，在继承后重写有需要时调用
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

    //单独考虑实体受到的魔法伤害值
    #region MagicalDamaged
    public virtual void GetMagicalDamagedBy(int _damage)
    //这里是数值的伤害的施加，而debuff的判定（需要传入敌方Stats）不放在这里，因为有些纯魔法攻击不会产生debuff
    {
        #region AttackedFX
        //受攻击的材质变化，使得有闪烁的动画效果
        entity.fx.StartCoroutine("FlashHitFX");

        //弹出伤害数值文本效果，玩家不弹
        if (entity.GetComponent<Player>() == null)
            entity.fx.CreatPopUpText(_damage.ToString(), Color.cyan);

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
    #endregion

    #region DecreaseDefence
    public void DecreaseDefenceBy(float _percentage, float _duration)
    //防御降低多少百分比，降低多久
    {
        //防御的降低，要先保存原有的数值
        defaultIntEvasion = evasionChance.GetValue();
        defaultIntArmor = physicalArmor.GetValue();
        defaultIntResistance = magicalResistance.GetValue();
        //降低防御值
        evasionChance.SetValue(Mathf.RoundToInt(evasionChance.GetValue() * (1 - _percentage)));
        physicalArmor.SetValue(Mathf.RoundToInt(physicalArmor.GetValue() * (1 - _percentage)));
        magicalResistance.SetValue(Mathf.RoundToInt(magicalResistance.GetValue() * (1 - _percentage)));

        //多久后恢复
        Invoke("ReturnToDefaultDefence", _duration);
    }
    public void ReturnToDefaultDefence()
    {
        //恢复数值
        evasionChance.SetValue(defaultIntEvasion);
        physicalArmor.SetValue(defaultIntArmor);
        magicalResistance.SetValue(defaultIntResistance);
    }
    #endregion

    #region FinalValues
    public virtual int GetNonCritPhysicalDamage()
    //得到不进行暴击判定的原始物理伤害
    {
        return primaryPhysicalDamage.GetValue() + 10 * strength.GetValue() + 5 * agility.GetValue();
    }
    public virtual int GetNonCritMagicalDamage()
    //得到不进行暴击判定的原始魔法伤害
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
    public virtual int GetFinalArmor()
    {
        //返回最终护甲值
        return physicalArmor.GetValue() + 2 * vitality.GetValue();
    }
    public virtual int GetFinalResistance()
    {
        //获取最终法术防御力
        return magicalResistance.GetValue() + 2 * intelligence.GetValue();
    }
    #endregion

    #region ChanceAnalyze
    private bool CanEvade()
    //是否闪避的检测是在被攻击方身上的脚本去进行
    {
        //通过随机数的方式，判断是否可以闪避
        if (UnityEngine.Random.Range(0, 100) <= GetFinalEvasionChance())
        {
            #region EvadeFX
            //文字弹出效果，玩家和怪物都弹
            entity.fx.CreatPopUpText("Miss", Color.yellow);
            //闪避的音效
            AudioManager.instance.PlaySFX(12, null);
            //闪避的粒子效果，在自己（受攻击者）身上
            fx.CreateHitFX00(this.transform);
            #endregion

            return true;
        }
        return false;
    }
    private bool CanCrit(EntityStats _attackingEntity)
    //是否暴击的检测也应当在被攻击方身上的脚本进行，不然无法判断是否被暴击，从而无法施加相应的效果
    {
        //注意Random使用的是Unity内的Random函数，此处通过随机数的方式，判断是否可以暴击
        if(UnityEngine.Random.Range(0,100) <= _attackingEntity.GetFinalCriticChance())
        {
            #region CritFX
            //受暴击的音效
            AudioManager.instance.PlaySFX(13, null);
            //被暴击的粒子效果，在自己（受攻击者）身上
            fx.CreateHitFX01(this.transform);
            #endregion

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
        if (_statType == StatType.fireDamage) { return fireAttackDamage.GetValue(); }
        if (_statType == StatType.iceDamage) { return iceAttackDamage.GetValue(); }
        if (_statType == StatType.lightningDamage) { return lightningAttackDamage.GetValue(); }
        if (_statType == StatType.evasion) { return GetFinalEvasionChance(); }
        if (_statType == StatType.armor) { return GetFinalArmor(); }
        if (_statType == StatType.resistance) { return GetFinalResistance(); }

        //注意这里返回空，不然会报错（因为所有的可能性都需要有返回值）
        return 0;
    }
    #endregion
}