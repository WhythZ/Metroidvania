using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
//此类的对象作为容器，表示人物的各种动作，每个新动作的脚本都要创建PlayerState类型变量并通过构造函数对其初始化；此处不需要继承MonoBehaviour
{
    #region Initialize
    //受哪个状态机控制
    protected PlayerStateMachine stateMachine;
    //动作的名称，即这个动作在Animator内相关联的bool值（状态判断）
    private string animBoolName;
    //动作所属对象
    protected Player player;
    //这里将此rb与Player.cs的rb等价链接起来，使得PlayerState子类中调用rb更加快捷（少写几个代码）
    protected Rigidbody2D rb;
    #endregion

    //水平速度输入
    public float xInput {  get; private set; }
    //竖直速度输入
    public float yInput {  get; private set; }

    //每个状态都对对应有的一个计时器
    protected float stateTimer;
    //每个状态都有的一个状态触发记录器
    protected bool stateActionFinished;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    //PlayerState类的构造函数，指明了确定一个动作状态需要三个东西：动作属于谁（Player），受哪个状态机控制，这个动作在Animator内相关联的bool值
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        //这样的话在PlayerState的子类中可以直接用比如rb.velocity.x调用水平速度相关信息，而无需用player.rb.velocity.x
        rb = player.rb;

        //赋值这个动作的激活状态为真
        player.anim.SetBool(animBoolName, true);

        //每次进入新的状态时，赋值这个触发为假
        stateActionFinished = false;
    }
  
    public virtual void Update()
    //在Player内使用其Update不断调用此函数，而非让PlayerState继承MonoBehavior并依靠其调用Update；继承MonoBehavior的类越少越好，方便管理
    {
        //每1s递减1单位数值
        stateTimer -= Time.deltaTime;
        //持续更新，将yVelocity参数赋值为当前的竖直速度
        player.anim.SetFloat("yVelocity", rb.velocity.y);
        //将水平速度与AD两个键位绑定，竖直速度与WS两键位绑定；移动速度可以被大多数动作访问到，故而放到基类中进行定义
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
    }
    
    public virtual void Exit()
    {
        //赋值这个动作的激活状态为假
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void TriggerWhenAnimationFinished()
    {
        //激活这个记录器，使得attackState转移到某种状态
        stateActionFinished = true;
    }
}
