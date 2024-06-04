using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    //用这个anim变量去接收和控制这个实体的子Sprite动画管理器
    public Animator anim { get; private set; }
    //接收实体的刚体Component
    public Rigidbody2D rb { get; private set; }
    //接收控制实体动画效果的脚本组件
    public EntityFX fx { get; private set; }
    //接收实体的碰撞组件，用于比如死亡后掉下平台（解除碰撞）
    public BoxCollider2D cd {  get; private set; }
    #endregion

    #region Collision
    [Header("Basic Collision Info")]
    //攻击碰撞范围（实体前方的一个圆）
    public Transform attackCheck;
    public float attackCheckRadius = 1f;

    //地面检测指标
    public bool isGround {  get; private set; }
    //地面检测；这个Transform类对象用于接收Unity内实体对象的子Sprite，用于单独操控碰撞检测线的位置，设置两个防止卡位等bug
    [SerializeField] protected Transform groundCheck_1;
    [SerializeField] protected Transform groundCheck_2;
    [SerializeField] protected float groundCheckDistance;
    //储存地面图层（这个属性需要手动赋予给每个Sprite）有哪些，便于后续为之做出相应反应
    [SerializeField] protected LayerMask whatIsGround;

    //墙壁检测，墙壁的图层沿用whatIsGround即可，地面和墙壁没啥区别
    public bool isWall {  get; private set; }
    [SerializeField] protected Transform wallCheck_1;
    [SerializeField] protected Transform wallCheck_2;
    [SerializeField] protected float wallCheckDistance;
    #endregion

    #region Movement
    [Header("Basic Movement Info")]
    //初始移动速度倍率
    public float moveSpeed = 10;

    //1为右，-1为左；默认面向方向为向右
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;
    #endregion

    #region Knockback
    [Header("Knockback Info")]
    //实体被攻击后的击退效果向量，即含有x和y两个效果分量
    [SerializeField] protected Vector2 knockbackDirVector;
    //记录击退的朝向
    protected int knockbackDir;
    //击退效果持续时间
    [SerializeField] protected float knockbackDuration = 0.07f;
    //默认设置false，否则受此布尔影响的SetVelocity函数无法运行；另一种不用设置默认值的方法详情见该函数处
    protected bool isKnocked = false;
    #endregion

    #region States
    //用于记录上一个状态的Animator内的parameter的名称，比如可以用于敌人死亡时保留上一个状态的动画
    public string lastAnimBoolName {  get; private set; }
    #endregion

    #region Events
    //用于记录实体转向这个事件，在Plip()函数处调用
    public System.Action onFlipped;
    #endregion

    protected virtual void Awake()
    //对象刚出生时调用且仅调用一次，相当于类的构造函数；比Start更早调用
    {
    }

    protected virtual void Start()
    {
        #region Components
        //链接rb到实体的刚体Component上
        rb = GetComponent<Rigidbody2D>();
        //接收这个实体的子Sprite动画管理器
        anim = GetComponentInChildren<Animator>();
        //接收控制实体动画效果的脚本组件
        fx = GetComponent<EntityFX>();
        //链接碰撞组件
        cd = GetComponent<BoxCollider2D>();
        #endregion
    }

    protected virtual void Update()
    {
        //不断更新实体的所有碰撞检测
        CollisionDetect();
        //确保实体的朝向正确
        FlipController();
        //被击退方向检测
        KnockbackDirDetect();
        //检测实体的死亡
        DieDetect();
    }

    #region Knockback
    protected virtual void KnockbackDirDetect()
    {
    }
    public IEnumerator HitKnockback()
    //在Damage()函数处以StartCoroutine("HitKnockback");的方式被调用
    //若是接收参数，则用比如StartCoroutine("BusyFor", _seconds);来调用
    {
        //这个bool值用于SetVelocity函数是否激活，防止速度向量相抵消
        isKnocked = true;

        //触发击退向量，含有朝反方向和上放的分量，朝玩家面对方向被击退
        rb.velocity = new Vector2(knockbackDir * knockbackDirVector.x, knockbackDirVector.y);
        //这个效果持续多久
        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }
    #endregion

    #region Collision
    public virtual void CollisionDetect()
    {
        //实际进行碰撞检测的代码
        bool isGround_1 = Physics2D.Raycast(groundCheck_1.position, Vector2.down, groundCheckDistance, whatIsGround);
        bool isGround_2 = Physics2D.Raycast(groundCheck_2.position, Vector2.down, groundCheckDistance, whatIsGround);
        //两根检测线只要有一根检测到地板，则视为人物在地板上
        isGround = isGround_1 || isGround_2;

        bool isWall_1 = Physics2D.Raycast(wallCheck_1.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        bool isWall_2 = Physics2D.Raycast(wallCheck_2.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        isWall = isWall_1 || isWall_2;
    }
    public virtual void OnDrawGizmos()
    //此函数用于可视化地画出各个检测线；此函数无需手动调用
    {
        //地面检测线；可以看到这条线是从groundCheck这个子Sprite中心画出的，而非从实体中心画出，这样可以更加灵活的进行碰撞检测
        Gizmos.DrawLine(groundCheck_1.position, new Vector3(groundCheck_1.position.x, groundCheck_1.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck_2.position, new Vector3(groundCheck_2.position.x, groundCheck_2.position.y - groundCheckDistance));

        //墙壁检测线，这里wallCheckDistance乘上了facingDir来确保墙壁检测线的转向
        Gizmos.DrawLine(wallCheck_1.position, new Vector3(wallCheck_1.position.x + wallCheckDistance * facingDir, wallCheck_1.position.y));
        Gizmos.DrawLine(wallCheck_2.position, new Vector3(wallCheck_2.position.x + wallCheckDistance * facingDir, wallCheck_2.position.y));

        //攻击范围的圆
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Velocity
    public virtual void SetVelocity(float _xVelocity, float _yVelocity)
    //当需要控制实体速度的时候调用此函数
    {
        //当不在被击退状态时，才速度正常；这种写法无需设置isKnocked的默认值
        if (isKnocked)
            return;
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }
    #endregion

    #region LastAnimBoolName
    public virtual void AssignLastAnimBoolName(string _lastAnimBoolName)
    {
        lastAnimBoolName = _lastAnimBoolName;
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        //反转实体
        transform.Rotate(0, 180, 0);
        //把方向的相关判断参数反向
        facingDir *= -1;
        facingRight = !facingRight;

        //当实体对象上有比如血条UI时，记录转向事件，并调用其对UI的特别的函数，若对象没有该类UI，则不记录，也不会调用UI脚本（防止报错）
        if (onFlipped != null)
            onFlipped();
    }
    public virtual void FlipController()
    {
        //非击退状态才可以依据移动速度转向，不然击退会自动转向，不自然
        if (isKnocked)
            return;
        else
        {
            //开始向右走且朝向为左时，反转
            if (rb.velocity.x > 0 && !facingRight)
            {
                Flip();
            }
            //开始向左走且朝向为右时，反转
            if (rb.velocity.x < 0 && facingRight)
            {
                Flip();
            }
        }
    }
    #endregion

    #region Die
    protected virtual void DieDetect()
    {
        //用于死亡检测的override
    }
    #endregion
}
