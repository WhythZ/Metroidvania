using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats, ISavesManager
{
    //其实直接用etity就好，基类里的entity已经获取了Player脚本了，但是这里还是习惯用player
    Player player;

    #region Skill
    [Header("Skill Stats")]
    //投掷剑技能的附加的额外伤害，故而飞剑的伤害是的玩家原本的伤害（含暴击判断等）加上这个额外数值
    //这样的好处是不用为剑的伤害额外计算暴击率等信息了，后面可能要改，先暂时这样
    public Stat swordExtraDamage;
    #endregion

    protected override void Start()
    {
        base.Start();

        //和player=entity等效
        player = PlayerManager.instance.player;
    }

    #region ChangePlayerStat
    public void EditPlayerStat(StatType _statType, int _modify)
    //用于在游进程戏中对人物进行加点处理，原则上只对能力属性值Attributes进行加点
    {
        //即在这项属性原来的值的基础上加上_modify的值，不论正负
        if (_statType == StatType.strength) { this.strength.SetValue(this.strength.GetValue() + _modify); }
        if (_statType == StatType.agility) { this.agility.SetValue(this.agility.GetValue() + _modify); }
        if (_statType == StatType.vitality) { this.vitality.SetValue(this.vitality.GetValue() + _modify); }
        if (_statType == StatType.intelligence) { this.intelligence.SetValue(this.intelligence.GetValue() + _modify); }
    }
    #endregion

    #region DamagedOverride
    public override void GetMagicalDamagedBy(int _damage)
    {
        //冲刺的时候不触发受击
        if (player.stateMachine.currentState == player.dashState)
            return;
        base.GetMagicalDamagedBy(_damage);
    }
    public override void GetPhysicalDamagedBy(int _damage)
    {
        //冲刺的时候不触发受击
        if (player.stateMachine.currentState == player.dashState)
            return;
        base.GetPhysicalDamagedBy(_damage);
    }
    #endregion

    #region DieOverride
    public override void StatsDie()
    {
        base.StatsDie();
    }
    #endregion

    #region ISaveManager
    public void LoadData(GameData _data)
    {
        //角色能力值
        this.strength.SetValue(_data.strength);
        this.agility.SetValue(_data.agility);
        this.vitality.SetValue(_data.vitality);
        this.intelligence.SetValue(_data.intelligence);

        //原始最大生命值（未经加成）
        this.originalMaxHealth.SetValue(_data.originalMaxHealth);

        //暴击相关属性（未经加成）
        this.criticPower.SetValue(_data.criticPower);
        this.criticChance.SetValue(_data.criticChance);

        //各项攻击力（未经加成）
        this.primaryPhysicalDamage.SetValue(_data.primaryPhysicalDamage);
        this.swordExtraDamage.SetValue(_data.swordExtraDamage);
        this.fireAttackDamage.SetValue(_data.fireAttackDamage);
        this.iceAttackDamage.SetValue(_data.iceAttackDamage);
        this.lightningAttackDamage.SetValue(_data.lightningAttackDamage);

        //闪避与防御（未经加成）
        this.evasionChance.SetValue(_data.evasionChance);
        this.physicalArmor.SetValue(_data.physicalArmor);
        this.magicalResistance.SetValue(_data.magicalResistance);
    }

    public void SaveData(ref GameData _data)
    //若游戏内进行了属性值的加点，则进行保存，如果只会对能力属性值进行加点，则只需要保存这些
    {
        //角色能力值
        _data.strength = this.strength.GetValue();
        _data.agility = this.agility.GetValue();
        _data.vitality = this.vitality.GetValue();
        _data.intelligence = this.intelligence.GetValue();

        //原始最大生命值（未经加成）
        _data.originalMaxHealth = this.originalMaxHealth.GetValue();

        //暴击相关属性（未经加成）
        _data.criticPower = this.criticPower.GetValue();
        _data.criticChance = this.criticChance.GetValue();

        //物理和法术攻击力（未经加成）
        _data.primaryPhysicalDamage = this.primaryPhysicalDamage.GetValue();
        _data.fireAttackDamage = this.fireAttackDamage.GetValue();
        _data.iceAttackDamage = this.iceAttackDamage.GetValue();
        _data.lightningAttackDamage = this.lightningAttackDamage.GetValue();

        //技能攻击力（未经加成）
        _data.swordExtraDamage = this.swordExtraDamage.GetValue();

        //闪避与防御（未经加成）
        _data.evasionChance = this.evasionChance.GetValue();
        _data.physicalArmor = this.physicalArmor.GetValue();
        _data.magicalResistance = this.magicalResistance.GetValue();
    }
    #endregion
}
