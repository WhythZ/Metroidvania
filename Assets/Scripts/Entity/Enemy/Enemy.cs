using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{
    #region States
    //链接到每个敌人类的状态机
    public EnemyStateMachine stateMachine { get; private set; }
    #endregion

    #region EnemyType
    [Header("EnemyType")]
    //记录怪物的种类
    public EnemyType enemyType;
    #endregion

    #region EnemyMove
    [Header("Enemy Move Info")]
    //怪物站立了一段时间后自动开始移动
    public float pauseTime = 0.5f;

    //怪物发现玩家后，以加速倍率移动（这个值是用来乘上moveSpeed的）
    public float battleSpeedMultiplier = 2f;
    //依据玩家位置决定battle状态下应当前往左边（-1）还是右边（1）行进
    public int battleMoveDir;

    //怪物与玩家重合位置的判定区域半径
    //public float overlapRegionRadius = 0.1f;
    #endregion

    #region Default
    private float defaultBattleSpeedMultiplier;
    #endregion

    #region Battle
    [Header("Battle Info")]
    public bool isPlayer;
    [SerializeField] protected Transform playerCheck;
    [SerializeField] protected float playerCheckDistance;
    [SerializeField] protected LayerMask whatIsPlayer;
    //进入攻击距离（新建一个距离检测）之后，可以进入攻击状态了；沿用Player的图层
    [SerializeField] protected Transform canAttackCheck;
    [SerializeField] protected float canAttackCheckDistance;
    public bool enterAttackRegion;
    //攻击冷却时间长度
    public float attackCooldown = 1.1f;
    //攻击冷却结束时，可以攻击
    public bool canAttack;
    //攻击冷却时间计时器
    protected float attackCooldownTimer;

    //持续一段时间在battleState而没有进行攻击的话，脱战
    public float quitBattleTime = 20f;
    //超出这个倍率乘上playerCheckDistance的距离同样脱战
    [SerializeField] protected float quitBattleDistanceRatio = 1.5f;
    //当发现过一次玩家或者被玩家攻击后后进入battle，只有当玩家离开限定范围或者一定时间内未攻击到玩家才会脱离锁定，否则无论经历什么状态转换，都应当回到battle
    public bool shouldEnterBattle = false;
    #endregion

    #region Stunned
    [Header("Stunned Info")]
    //怪物被主角格挡弹反后进入眩晕，此处为眩晕时长；弹反的击退调用普通攻击击退StartCoroutine("HitKnockback");即可
    public float stunnedDuration = 1f;
    //可以被弹反的信号，受专属函数控制，为AnimationTrigger脚本提供接口
    private bool canBeStunned = false;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        #region States
        stateMachine = new EnemyStateMachine();
        #endregion
    }

    protected override void Start()
    {
        base.Start();

        #region Default
        defaultBattleSpeedMultiplier = battleSpeedMultiplier;
        #endregion
    }

protected override void Update()
    {
        base.Update();

        //此处是通过MonoBehavior的Update函数来不断调用EnemyState类中的Update函数，不断刷新怪物状态
        stateMachine.currentState.Update();

        //玩家检测
        PlayerCollisionDetect();

        //控制攻击冷却等
        AttackController();

        //是否进入仇恨状态检测
        AggressiveDetect();
    }

    #region KnockbackOverride
    protected override void KnockbackDirDetect()
    //对于Enemy而言，应当是被玩家攻击，击退方向自然是玩家的朝向
    {
        base.KnockbackDirDetect();

        //如果人物从左侧攻击怪物，击退方向为右边；反之为左
        knockbackDir = PlayerManager.instance.player.facingDir;
    }
    #endregion

    #region Stunned
    public void OpenCounterAttackWindow()
    {
        //开启能被弹反眩晕的状态
        canBeStunned = true;
    }
    public void CloseCounterAttackWindow()
    {
        //关闭这个状态
        canBeStunned = false;
    }
    public virtual bool WhetherCanBeStunned()
    {
        //如果被弹反成功了，则canBeStunned返回true，而多写一个函数的原因是为了调用一次CloseCounterAttackWindow();
        //同时这个函数可在子类敌人脚本中被override的同时调用stateMachine.ChangeState()函数
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }
    #endregion

    #region SlowEntityOverride
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        //特性的减速，写在前面
        battleSpeedMultiplier *= (1 - _slowPercentage);

        base.SlowEntityBy(_slowPercentage, _slowDuration);
    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        //恢复速度
        battleSpeedMultiplier = defaultBattleSpeedMultiplier;
    }
    #endregion


    #region Attack
    public virtual void AttackController()
    {
        //计时器开始计时
        attackCooldownTimer -= Time.deltaTime;

        //如果计时器为负，则可以在玩家进入攻击范围后攻击
        if(attackCooldownTimer < 0)
        {
            canAttack = true;
        }
        else if(attackCooldownTimer > 0)
        {
            canAttack = false;
        }
    }

    //攻击冷却刷新
    public virtual void AttackCooldownRefresher() => attackCooldownTimer = attackCooldown;
    
    //这个值即怪物脱战距离
    public virtual float GetQuitBattleDisance() => playerCheckDistance * quitBattleDistanceRatio;
    #endregion  

    #region AggressiveDetect
    protected virtual void AggressiveDetect()
    {
        if(isKnocked)
        {
            //当怪物被攻击后，进入仇恨状态
            shouldEnterBattle = true;
        }
    }
    #endregion

    #region PlayerDetect
    public virtual void PlayerCollisionDetect()
    {
        isPlayer = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer);
        enterAttackRegion = Physics2D.Raycast(canAttackCheck.position, Vector2.right * facingDir, canAttackCheckDistance, whatIsPlayer);
    }
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //为玩家检测线设置一个特别的颜色
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance * facingDir, playerCheck.position.y));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(canAttackCheck.position, new Vector3(canAttackCheck.position.x + canAttackCheckDistance * facingDir, canAttackCheck.position.y));
    }
    #endregion

    #region AnimationTrigger
    public void AnimationTrigger() => stateMachine.currentState.TriggerWhenAnimationFinished();
    //当此函数被调用时（即攻击动作结束的时候），返回调用当前状态的TriggerWhenAnimationFinished()函数的结果；此语句等价于下面这句
    //public void AnimationTrigger(){stateMachine.currentState.TriggerWhenAnimationFinished();}
    #endregion
}
