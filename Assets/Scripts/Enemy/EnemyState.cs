using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    #region Initialize
    //受哪个状态机控制
    protected EnemyStateMachine stateMachine;
    //动作的名称，即这个动作在Animator内相关联的bool值（状态判断）
    private string animBoolName;
    //动作所属对象
    protected Enemy enemyBase;
    //这里将此rb与Enemy.cs的rb等价链接起来，使得EnemyState子类中调用rb更加快捷（少写几个代码）
    protected Rigidbody2D rb;
    #endregion

    //每个状态都对对应有的一个计时器
    protected float stateTimer;
    //每个状态都有的一个状态触发记录器
    protected bool stateActionFinished;

    //这个已经不需要了，直接用PlayerManager.instance.transform.position即可
    //记录玩家的位置（注意是Transform数据类型）
    //public Transform playerPos;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        //这样的话在EnemyState的子类中可以直接用比如rb.velocity.x调用水平速度相关信息，而无需用enemy.rb.velocity.x
        rb = enemyBase.rb;

        //赋值这个动作的激活状态为真
        enemyBase.anim.SetBool(animBoolName, true);

        //每次进入新的状态时，赋值这个触发为假
        stateActionFinished = false;
    }

    public virtual void Update()
    {
        //每1s递减1单位数值
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        //赋值这个动作的激活状态为假
        enemyBase.anim.SetBool(animBoolName, false);
    }

    public virtual void TriggerWhenAnimationFinished()
    {
        //激活这个记录器，使得attackState转移到某种状态
        stateActionFinished = true;
    }
}