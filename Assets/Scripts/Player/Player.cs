using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Player : Entity
{
    #region States
    public PlayerStateMachine stateMachine {  get; private set; }
    //这个人物受到这个定义的状态机控制
    public PlayerIdleState idleState { get; private set; }
    //引入人物的站立状态
    public PlayerMoveState moveState { get; private set; }
    //引入人物的移动状态
    public PlayerJumpState jumpState { get; private set; }
    //引入人物的跳跃状态
    public PlayerAirState airState { get; private set; }
    //引入人物的坠落状态
    public PlayerDashState dashState { get; private set; }
    //引入人物的冲刺状态
    public playerWallSlideState wallSlideState { get; private set; }
    //引入人物的滑墙状态
    public PlayerWallJumpState wallJumpState { get; private set; }
    //引入人物的墙跳状态
    public PlayerPrimaryAttack primaryAttack { get; private set; }
    //引入人物的攻击状态
    public PlayerCounterAttackState counterAttackState { get; private set; }
    //引入人物的弹反状态，这个状态控制着两个相关的parameters
    public PlayerAimSwordState aimSwordState { get; private set; }
    //引入人物的瞄准状态
    public PlayerThrowSwordState throwSwordState { get; private set;}
    //引入人物的投掷状态
    public PlayerDeadState deadState { get; private set; }
    //引入死亡状态
    #endregion

    #region Components
    //记录对象的数据统计脚本
    public PlayerStats sts {  get; private set; }
    #endregion

    #region Movement
    [Header("Player Movement Info")]
    //人物在空中的移动速度是moveSpeed的小于一倍
    public float airMoveSpeedRate = 0.9f;
    #endregion

    #region Jump
    [Header("Jump Info")]
    //初始跳跃力
    public float jumpForce = 15;
    //人物剩余的可跳跃次数，人物最多可以二段跳
    public int jumpNum = 2;
    #endregion

    #region Skills
    //使得代码更简洁，不用PlayerSkillManager.instance.xxx，直接player.skill.instance.xxx即可
    public PlayerSkillManager skill {  get; private set; }
    //记录是否已经投掷出去了剑，防止无限投掷，在GroundedState中（即投掷能力的入口处）检测是否已经创建过剑Prefab
    //那里的player.assignedSword可以当bool值使用
    public GameObject assignedSword {  get; private set; }
    #endregion

    #region Dash
    [Header("Dash Info")]
    //冲刺时间默认0.2秒，即移动速度乘上dashSpeed倍率的持续时长
    public float dashDuration = 0.2f;
    //冲刺速度要比moveSpeed大，不然不叫冲刺了，默认为26
    public float dashSpeed = 26;
    //在空中冲刺过一次后，即使冷却时间到了也不能第二次冲刺
    public bool canDash {  get; private set; } = true;

    //有了PlayerSkillManager管理归属Skill父类的DashSkill，其内自有相关可调用内容，故无需下列变量
    //冷却时间长度
    //public float dashCooldown = 0.6f;
    //冷却时间计时器
    //private float dashCooldownTimer;
    #endregion

    #region AttackDetails
    [Header("Attack Details")]
    //跑动进入攻击状态时维持原来速度（惯性感）的时间
    public float runIntoAttackInertiaDuration = 0.1f;
    //储存不同段攻击的不同位移的向量数组
    public Vector2[] attackMovement;

    //弹反攻击能触发的有效时间
    public float counterAttackDuration = 0.3f;
    #endregion

    #region WallSlide
    [Header("WallSlide Info")]
    //滑墙的速度倍率
    public float slideSpeed = 0.9f;
    //滑墙的加速向下速度倍率（大于普通滑墙速度倍率）
    public float biggerSlideSpeed = 0.99f;
    //墙跳反向水平速度施加时长
    public float wallJumpReverseSpeedDuration = 0.1f;
    #endregion

    #region CDPlayer
    [Header("CDPlayer")]
    //生成随身听所用的随身听预制体
    public GameObject cdPlayerPrefab;
    //用于存储生成后的随身听对象，用于防止生成多个随身听预制体
    public GameObject assignedCDPlayer {  get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        #region States
        //新建一个状态机
        stateMachine = new PlayerStateMachine();
        //传入Player本身，使用的状态机，和这个动作在Unity内Animator处对应判断Parameter名称"Idle"，完成这个PlayerState即"idleState"的构造
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        //确保PlayerState及其子类影响的player是这个脚本里的Player；将这个人物状态的控制与Animator内Move参数绑定
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        //初始化跳跃状态
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        //注意这里绑定的也是Jump参数，因为没什么区别
        airState = new PlayerAirState(this, stateMachine, "Jump");
        //初始化冲刺状态
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        //初始化滑墙状态
        wallSlideState = new playerWallSlideState(this, stateMachine, "WallSlide");
        //初始化墙跳状态
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "WallJump");
        //初始化第一段攻击
        primaryAttack = new PlayerPrimaryAttack(this, stateMachine, "Attack");
        //初始化防御反击
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        //初始化瞄准动作
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        //初始化投掷状态
        throwSwordState = new PlayerThrowSwordState(this, stateMachine, "ThrowSword");
        //初始化死亡状态
        deadState = new PlayerDeadState(this, stateMachine, "Dead");
        #endregion
    }

    protected override void Start()
    {
        base.Start();

        #region Components
        //数据统计脚本
        sts = GetComponent<PlayerStats>();
        #endregion

        //用站立状态初始化玩家的状态机
        stateMachine.Initialize(idleState);
        //简化代码
        skill = PlayerSkillManager.instance;
    }

    protected override void Update()
    {
        #region GamePause
        //游戏处于暂停状态的时候，不执行下面的任何语句
        if (Time.timeScale == 0)
            return;
        #endregion

        base.Update();

        //此处是通过MonoBehavior的Update函数来不断调用PlayerState类中的Update函数，不断刷新人物状态
        stateMachine.currentState.Update();
        //控制人物的冲刺状态
        DashController();
        //控制人物的随身听
        CDPlayerController();
    }

    #region Dash
    private void DashController()
    {
        //计时器开始计时
        //dashCooldownTimer -= Time.deltaTime;

        //不能从攻击，瞄准与投掷状态冲刺
        if (stateMachine.currentState != primaryAttack && stateMachine.currentState != aimSwordState && stateMachine.currentState != throwSwordState)
        {
            //冲刺可以从任意允许的状态进入开始，故而放在此处Update里赋予其高优先级；只要按下左shift，且冷却时间结束，便进入冲刺状态；
            //注意这里使用了PlayerSkillManager
            if (Input.GetKeyDown(KeyCode.LeftShift) && skill.dashSkill.WhetherCanUseSkill() && canDash)
            {
                stateMachine.ChangeState(dashState);
            }
        }
    }
    #endregion

    #region FlipControllerOverride
    public override void FlipController()
    {
        if (isKnocked)
            return;
        else
        {
            //你妈的，这个状态直接给他特判掉，我倒看你tm还出不出问题！
            if(stateMachine.currentState != wallSlideState)
            {
                //这里新增一个xInput限制，防止莫名的速度增量产生的转向问题
                if ((stateMachine.currentState.xInput > 0) && (rb.velocity.x > 0) && !facingRight)
                {
                    Flip();
                }
                if ((stateMachine.currentState.xInput < 0) && (rb.velocity.x < 0) && facingRight)
                {
                    Flip();
                }
            }
        }
    }
    #endregion

    #region Sword
    public void AssignNewSword(GameObject _newSword)
    {
        //记录一下新建了一个剑Prefab，在CreateSword()函数中被调用一次
        assignedSword = _newSword;
    }
    public void ClearAssignedSword()
    {
        //销毁多余的剑Prefab
        Destroy(assignedSword);
    }
    #endregion

    #region CDPlayer
    private void CDPlayerController()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //若按下召唤键，之前生成过随身听，则清除现在的随身听；此条if一定要在另一个if的前面，不然刚生成的prefab会立刻被清除掉
            if (assignedCDPlayer != null)
            {
                //音效
                AudioManager.instance.PlaySFX(7, null);

                //防止生成多个随身听
                Destroy(assignedCDPlayer);
            }
            //若按下召唤键，之前没生成过随身听，则创建一个新的
            if(assignedCDPlayer == null)
            {
                //音效
                AudioManager.instance.PlaySFX(7, null);

                //初始化并生成一个随身听
                GameObject _newCDPlayer = Instantiate(cdPlayerPrefab, transform.position, transform.rotation);

                //记录一下，创建了一个新的随身听，防止无限召唤
                assignedCDPlayer = _newCDPlayer;
            }
        }
    }
    #endregion

    #region DieOverride
    protected override void DieDetect()
    {
        //若人物血量小于等于零，触发PlayerStats处的死亡函数
        if (sts.currentHealth <= 0)
        {
            //进入死亡状态
            stateMachine.ChangeState(deadState);
        }
    }
    #endregion

    #region Accessibility
    public void CanDashSetting(bool _bool)
    {
        //我不想让这个变量在Unity中可以操纵，但又需要这个变量在其他脚本中被操纵赋值，故用单独一个函数控制
        canDash = _bool;
    }
    #endregion

    #region AttackAnimationRelatedScripts
    public void AnimationTrigger() => stateMachine.currentState.TriggerWhenAnimationFinished();
    //当此函数被调用时（即攻击动作结束的时候），返回调用当前状态的TriggerWhenAnimationFinished()函数的结果；此语句等价于下面这句
    //public void AnimationTrigger(){stateMachine.currentState.TriggerWhenAnimationFinished();}
    #endregion
}
