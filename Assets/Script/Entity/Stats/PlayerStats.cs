using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats, ISavesManager
{
    //��ʵֱ����etity�ͺã��������entity�Ѿ���ȡ��Player�ű��ˣ��������ﻹ��ϰ����player
    Player player;

    #region Skill
    [Header("Skill Stats")]
    //Ͷ�������ܵĻ����˺�
    public Stat swordDamage;
    //��������˺�
    public Stat fireballDamage;
    //��������˺�
    public Stat iceballDamage;
    #endregion

    protected override void Start()
    {
        base.Start();

        //��player=entity��Ч
        player = PlayerManager.instance.player;
    }

    #region DamagedOverride
    public override void GetMagicalDamagedBy(int _damage)
    {
        //��̵�ʱ�򲻴����ܻ�
        if (player.stateMachine.currentState == player.dashState)
            return;
        base.GetMagicalDamagedBy(_damage);
    }
    public override void GetPhysicalDamagedBy(int _damage)
    {
        //��̵�ʱ�򲻴����ܻ�
        if (player.stateMachine.currentState == player.dashState)
            return;
        base.GetPhysicalDamagedBy(_damage);
    }
    #endregion

    #region ISaveManager
    public void LoadData(GameData _data)
    {
        //��ɫ����ֵ
        this.strength.SetValue(_data.strength);
        this.agility.SetValue(_data.agility);
        this.vitality.SetValue(_data.vitality);
        this.intelligence.SetValue(_data.intelligence);

        //ԭʼ�������ֵ��δ���ӳɣ�
        this.originalMaxHealth.SetValue(_data.originalMaxHealth);

        //����������ԣ�δ���ӳɣ�
        this.criticPower.SetValue(_data.criticPower);
        this.criticChance.SetValue(_data.criticChance);

        //�����ͷ�����������δ���ӳɣ�
        this.primaryPhysicalDamage.SetValue(_data.primaryPhysicalDamage);
        this.fireAttackDamage.SetValue(_data.fireAttackDamage);
        this.iceAttackDamage.SetValue(_data.iceAttackDamage);
        this.lightningAttackDamage.SetValue(_data.lightningAttackDamage);

        //���ܹ�������δ���ӳɣ�
        this.swordDamage.SetValue(_data.swordDamage);
        this.fireballDamage.SetValue(_data.fireballDamage);
        this.iceballDamage.SetValue(_data.iceballDamage);

        //�����������δ���ӳɣ�
        this.evasionChance.SetValue(_data.evasionChance);
        this.physicalArmor.SetValue(_data.physicalArmor);
        this.magicalResistance.SetValue(_data.magicalResistance);
    }

    public void SaveData(ref GameData _data)
    //����Ϸ�ڽ���������ֵ�ļӵ㣬����б��棬���ֻ�����������ֵ���мӵ㣬��ֻ��Ҫ������Щ
    {
        //��ɫ����ֵ
        _data.strength = this.strength.GetValue();
        _data.agility = this.agility.GetValue();
        _data.vitality = this.vitality.GetValue();
        _data.intelligence = this.intelligence.GetValue();

        //ԭʼ�������ֵ��δ���ӳɣ�
        _data.originalMaxHealth = this.originalMaxHealth.GetValue();

        //����������ԣ�δ���ӳɣ�
        _data.criticPower = this.criticPower.GetValue();
        _data.criticChance = this.criticChance.GetValue();

        //�����ͷ�����������δ���ӳɣ�
        _data.primaryPhysicalDamage = this.primaryPhysicalDamage.GetValue();
        _data.fireAttackDamage = this.fireAttackDamage.GetValue();
        _data.iceAttackDamage = this.iceAttackDamage.GetValue();
        _data.lightningAttackDamage = this.lightningAttackDamage.GetValue();

        //���ܹ�������δ���ӳɣ�
        _data.swordDamage = this.swordDamage.GetValue();
        _data.fireballDamage = this.fireballDamage.GetValue();
        _data.iceballDamage = this.iceballDamage.GetValue();

        //�����������δ���ӳɣ�
        _data.evasionChance = this.evasionChance.GetValue();
        _data.physicalArmor = this.physicalArmor.GetValue();
        _data.magicalResistance = this.magicalResistance.GetValue();
    }
    #endregion
}