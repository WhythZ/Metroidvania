using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    #region States
    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeadState deadState { get; private set; }
    #endregion

    #region Components
    public SlimeStats sts { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        #region States
        //注意，多传入了一个this
        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        //因为进入战斗状态后，先要接近玩家，即走过去，故先套用Move参数
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SlimeDeadState(this, stateMachine, "Dead", this);
        #endregion
    }

    protected override void Start()
    {
        base.Start();

        #region Components
        sts = GetComponent<SlimeStats>();
        #endregion

        //用站立状态初始化怪物的状态机
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        //战斗状态锁定敌人位置
        BattleMoveDirCheck();
        //战斗状态中要保持facingDir与battleMoveDir保持一致
        KeepDirectionSame();
    }

    #region StunnedOverride
    public override bool WhetherCanBeStunned()
    //这个状态转换在这里而不是在attackState里，是因为反正CounterAttackWindow只能在attackState的动画里被调用，在这里或那里切换状态都没差啦
    //同时，在Slime脚本内还能重写这个函数
    {
        if (base.WhetherCanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);

            return true;
        }
        return false;
    }
    #endregion

    #region BattleMove
    //战斗状态锁定敌人位置
    public void BattleMoveDirCheck()
    {
        //不应在attackState中被检测，防止攻击的时候突然转向，因为在此状态会检测battleMoveDir和facingDir是否统一，否则规整后者为前者，
        if (stateMachine.currentState == battleState || stateMachine.currentState == idleState)
        {
            if (PlayerManager.instance.player.transform.position.x > (rb.position.x))
            {
                battleMoveDir = 1;
            }
            if (PlayerManager.instance.player.transform.position.x < (rb.position.x))
            {
                battleMoveDir = -1;
            }

            //不能直接判定playerPos.position.x == rb.position.x，因为两者位置由于浮点精度或每帧位置的影响不可能完全相同
        }
    }
    public void KeepDirectionSame()
    {
        //战斗状态中要保持facingDir与battleMoveDir保持一致，防止出现被打飞后莫名转向的现象
        if ((stateMachine.currentState == battleState) || (stateMachine.currentState == attackState) || (stateMachine.currentState == stunnedState))
        //if的使用使得退出battle状态后facingDir回归原来的判断，也就是说battleMoveDir只有在battle或者attack或者stunned状态下使用
        {
            if (facingDir != battleMoveDir)
            {
                Flip();
            }
        }
    }
    #endregion

    #region DieOverride
    protected override void DieDetect()
    {
        base.DieDetect();

        if (sts.currentHealth <= 0)
        {
            sts.StatsDie();
        }
    }
    public override void EntityDie()
    {
        base.EntityDie();

        stateMachine.ChangeState(deadState);
    }
    #endregion
}
