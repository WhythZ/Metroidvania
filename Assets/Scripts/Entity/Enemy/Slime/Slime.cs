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

    #region SlimeSplit
    [Header("Slime Split")]
    //史莱姆分裂后产生多少下一级史莱姆
    public int splitSlimeCount;
    //分裂出来的下一级史莱姆，最小级史莱姆不分裂
    public GameObject splitSlime;
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

    #region SlimeSplit
    /*public void CreatSplitSlime(GameObject _splitPrefab, int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            //随机位置生成下一级史莱姆
            float _xPos = UnityEngine.Random.Range(-2f, 2f);

            //分裂生成下一级史莱姆
            GameObject _newSlime = Instantiate(_splitPrefab, transform.position + new Vector3(_xPos, 0), Quaternion.identity);
        }
    }*/

    public void CreatSplitSlime(GameObject _splitPrefab, int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            //分裂生成下一级史莱姆
            GameObject _newSlime = Instantiate(_splitPrefab, transform.position, Quaternion.identity);
            //给予分裂出来的史莱姆一个任意方向的初速度
            _newSlime.GetComponent<Slime>().SetupSplitVector();
        }
    }
    public void SetupSplitVector()
    //给分裂出来的史莱姆一个初速度，这样有动感
    {
        //给予一个范围内随机的速度向量，这里是框定范围
        float dx = Random.Range(-5, 5);
        float dy = Random.Range(6, 9);

        //当实体处于isKnocked状态的时候，施加的速度向量不会被原本的SetVelocity的速度所覆盖，故而达到弹开的速度效果
        isKnocked = true;

        //向量的施加
        GetComponent<Rigidbody2D>().velocity = new Vector2(dx, dy);
        
        //两秒后把isKnocked赋值为false
        Invoke("CancelSliptVector", 1f);
    }
    private void CancelSliptVector() => isKnocked = false;
    #endregion

    #region DieOverride
    protected override void DieDetect()
    {
        base.DieDetect();

        if (sts.currentHealth <= 0)
        {
            stateMachine.ChangeState(deadState);
        }
    }
    #endregion
}
