using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityBuffs : MonoBehaviour
//用于控制实体的Buff状态
{
    #region Components
    private Entity entity;
    private EntityStats sts;
    private EntityFX fx;
    #endregion

    #region Buffs
    [Header("Buffs")]
    //燃烧状态，效果时间内持续掉血
    public Buff_Ignited ignited;
    //冰冻状态，效果时间内速度减慢
    public Buff_Chilled chilled;
    //眩晕状态，效果时间内防御降低
    public Buff_Shocked shocked;
    #endregion

    #region Timers
    //灼烧状态计时器
    private float ignitedTimer;
    private float ignitedDamageTimer;
    //冰冻状态计时器
    private float chilledTimer;
    //眩晕状态计时器
    private float shockedTimer;
    #endregion

    private void Start()
    {
        #region Components
        entity = GetComponent<Entity>();
        sts = GetComponent<EntityStats>();
        fx = GetComponent<EntityFX>();
        #endregion

        //初始时清空所有Buffs
        ClearAllBuffs();
    }

    private void Update()
    {
        //检测各种Buff
        BuffsDetector();
    }

    #region DetectBuffs
    private void BuffsDetector()
    {
        if (ignited.GetStatus())
        {
            //启用计时器的刷新
            ignitedTimer -= Time.deltaTime;
            ignitedDamageTimer -= Time.deltaTime;

            //灼烧伤害的施加
            if (ignitedDamageTimer < 0)
            {
                //百分比烧血
                int _ignitedDamage = Mathf.RoundToInt(sts.GetFinalMaxHealth() * ignited.burnHealthPercentage);
                //这里使用的函数仅对数值产生影响，而且其内还会触发各种效果，同时不会进行Buff应用的检测，减少无效运算
                this.sts.GetMagicalDamagedBy(_ignitedDamage);

                //重置冷却时长，达到每隔一段时间进行灼烧的效果
                ignitedDamageTimer = ignited.damageCooldown;
            }

            //退出燃烧状态
            if (ignitedTimer < 0)
            {
                ignited.SetStatus(false);
            }
        }

        if (chilled.GetStatus())
        //冷冻状态有减速效果，会在刚进入此状态时调用一次
        {
            chilledTimer -= Time.deltaTime;

            //退出冰冻状态
            if (chilledTimer < 0)
            {
                chilled.SetStatus(false);
            }
        }

        if (shocked.GetStatus())
        //眩晕状态有防御减免效果，会在刚进入此状态时调用一次
        {
            shockedTimer -= Time.deltaTime;

            //退出眩晕状态
            if (shockedTimer < 0)
            {
                shocked.SetStatus(false);
            }
        }
    }
    #endregion

    #region ApplyBuffs
    public virtual void CheckBuffsFrom(EntityStats _entity)
    {
        #region Evaluation
        //存储攻击自己的实体的魔法元素伤害数据
        int _fireDmg = _entity.fireAttackDamage.GetValue();
        int _iceDmg = _entity.iceAttackDamage.GetValue();
        int _lightDmg = _entity.lightningAttackDamage.GetValue();

        //只要有这个类型的魔法伤害，则施加这个buff
        bool _canApplyIgnite = (_fireDmg > 0);
        bool _canApplyChill = (_iceDmg > 0);
        bool _canApplyShock = (_lightDmg > 0);
        #endregion

        //施加Buffs
        ApplyBuffs(_canApplyIgnite, _canApplyChill, _canApplyShock);
    }
    public virtual void ApplyBuffs(bool _ignited, bool _chilled, bool _shocked)
    {
        #region Evaluation
        //检测原来是否已经有了相应类型的Buff，若有则不应当再多调用一次其效果（会导致比如减速效果的累加，导致走不动）
        bool _canCallIgnited = _ignited;
        if (ignited.GetStatus() == true)
        {
            _canCallIgnited = false;
        }

        bool _canCallChilled = _chilled;
        if (chilled.GetStatus() == true)
        {
            _canCallChilled = false;
        }

        bool _canCallShocked = _shocked;
        if (shocked.GetStatus() == true)
        {
            _canCallShocked = false;
        }
        #endregion

        //赋予Buffs及其效果
        if (_canCallIgnited)
        {
            //检测下Buff被赋予了几次
            //Debug.Log(entity.name + " Get Ignited");

            //激活状态
            ignited.SetStatus(true);
            
            //刷新计时器
            ignitedTimer = ignited.duration;
            ignitedDamageTimer = ignited.damageCooldown;

            //调用状态效果
            fx.InvokeIgnitedFXFor(ignited.duration);
        }

        if (_canCallChilled)
        {
            //激活状态
            chilled.SetStatus(true);

            //刷新计时器
            chilledTimer = chilled.duration;

            //应用冷冻的减速，脱离冷冻状态后恢复原有速度
            entity.SlowEntityBy(chilled.slowPercentage, chilled.duration);

            //调用状态效果
            fx.InvokeChilledFXFor(chilled.duration);
        }

        if (_canCallShocked)
        {
            //激活状态
            shocked.SetStatus(true);

            //刷新计时器
            shockedTimer = shocked.duration;

            //防御的降低，脱离眩晕状态后恢复原有数值
            sts.DecreaseDefenceBy(shocked.defenceDecreasePercentage, shocked.duration);

            //调用状态效果
            fx.InvokeShockedFXFor(shocked.duration);
        }
    }
    #endregion

    #region ClearBuffs
    public void ClearAllBuffs()
    {
        //清除所有Buffs
        ignited.SetStatus(false);
        chilled.SetStatus(false);
        shocked.SetStatus(false);
    }
    #endregion
}
