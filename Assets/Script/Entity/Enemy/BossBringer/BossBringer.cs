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
        //ע�⣬�ഫ����һ��this
        idleState = new BossBringerIdleState(this, stateMachine, "Idle", this);
        moveState = new BossBringerMoveState(this, stateMachine, "Move", this);
        //��Ϊ����ս��״̬����Ҫ�ӽ���ң����߹�ȥ����������Move����
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

        //��վ��״̬��ʼ�������״̬��
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        //���´�����ȴ
        //teleportTimer -= Time.deltaTime;
        //ս��״̬��������λ��
        BattleMoveDirCheck();
        //ս��״̬��Ҫ����facingDir��battleMoveDir����һ��
        KeepDirectionSame();
    }

    #region StunnedOverride
    public override bool WhetherCanBeStunned()
    //���״̬ת���������������attackState�����Ϊ����CounterAttackWindowֻ����attackState�Ķ����ﱻ���ã�������������л�״̬��û����
    //ͬʱ����BossBringer�ű��ڻ�����д�������
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
    //ս��״̬��������λ��
    public void BattleMoveDirCheck()
    {
        //��Ӧ��attackState�б���⣬��ֹ������ʱ��ͻȻת����Ϊ�ڴ�״̬����battleMoveDir��facingDir�Ƿ�ͳһ�������������Ϊǰ�ߣ�
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
       
            //����ֱ���ж�playerPos.position.x == rb.position.x����Ϊ����λ�����ڸ��㾫�Ȼ�ÿ֡λ�õ�Ӱ�첻������ȫ��ͬ
        }
    }
    public void KeepDirectionSame()
    {
        //ս��״̬��Ҫ����facingDir��battleMoveDir����һ�£���ֹ���ֱ���ɺ�Ī��ת�������
        if ((stateMachine.currentState == battleState) || (stateMachine.currentState == attackState) || (stateMachine.currentState == stunnedState))
        //if��ʹ��ʹ���˳�battle״̬��facingDir�ع�ԭ�����жϣ�Ҳ����˵battleMoveDirֻ����battle����attack����stunned״̬��ʹ��
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
