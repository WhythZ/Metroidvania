using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BossBringer : Enemy
{
    [Header("Teleport details")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSizes;
    //public float teleportTimer;

    #region States
    public BossBringerIdleState idleState { get; private set; }
    public BossBringerMoveState moveState { get; private set; }
    public BossBringerBattleState battleState { get; private set; }
    public BossBringerAttackState attackState { get; private set; }
    public BossBringerTeleportState teleportState { get; private set; }
    public BossBringerSpellCastState spellCastState { get; private set; }
    public BossBringerStunnedState stunnedState { get; private set; }
    public BossBringerDeadState deadState { get; private set; }
    #endregion

    #region Components
    public BossBringerStats sts {  get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        #region States
        //注意，多传入了一个this
        idleState = new BossBringerIdleState(this, stateMachine, "Idle", this);
        moveState = new BossBringerMoveState(this, stateMachine, "Move", this);
        //因为进入战斗状态后，先要接近玩家，即走过去，故先套用Move参数
        battleState = new BossBringerBattleState(this, stateMachine, "Move", this);
        attackState = new BossBringerAttackState(this, stateMachine, "Attack", this);
        teleportState = new BossBringerTeleportState(this, stateMachine, "Teleport", this);
        spellCastState = new BossBringerSpellCastState(this, stateMachine, "SpellCast", this);
        stunnedState = new BossBringerStunnedState(this, stateMachine, "Stunned", this);
        deadState = new BossBringerDeadState(this, stateMachine, "Dead", this);
        #endregion
    }

    protected override void Start()
    {
        base.Start();

        #region Components
        sts = GetComponent<BossBringerStats>();
        #endregion

        //用站立状态初始化怪物的状态机
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        //更新传送冷却
        //teleportTimer -= Time.deltaTime;
        //战斗状态锁定敌人位置
        BattleMoveDirCheck();
        //战斗状态中要保持facingDir与battleMoveDir保持一致
        KeepDirectionSame();
    }

    #region StunnedOverride
    public override bool WhetherCanBeStunned()
    //这个状态转换在这里而不是在attackState里，是因为反正CounterAttackWindow只能在attackState的动画里被调用，在这里或那里切换状态都没差啦
    //同时，在BossBringer脚本内还能重写这个函数
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
        if(stateMachine.currentState == battleState || stateMachine.currentState == idleState)
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
            if(facingDir != battleMoveDir)
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

        if(sts.currentHealth <= 0)
        {
            Debug.Log(sts.currentHealth); 
            stateMachine.ChangeState(deadState);
        }
    }
    #endregion

    #region TeleportDetect

    public void FindPosition()
    {
        float playerPosX = PlayerManager.instance.player.transform.position.x;
        float playerPosY = PlayerManager.instance.player.transform.position.y;

        float x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        float y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);



        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));   

        if (!GroundBelow() || SomethingIsArround())
        {
            Debug.Log("Looking for new position");
            FindPosition();
        }
    }

    protected RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    protected bool SomethingIsArround() => Physics2D.BoxCast(transform.position, surroundingCheckSizes, 0, Vector2.zero, 0, whatIsGround);

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSizes);
    }
    #endregion
}
