using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Controller : MonoBehaviour
//为了防止剑粘到玩家或者其他物体身上，应该为剑单独设置一个图层Sword，并在ProjectSettings的Physics2D内将其仅勾选Ground和Enemy
{
    #region Components
    private BoxCollider2D cd;
    private Rigidbody2D rb;
    private Animator anim;
    #endregion

    //决定剑尖的指向方向是否能随速度改变
    private bool canRotate = true;
    //是否处于返回状态，若是，则应当返回到玩家处
    private bool isReturning = false;
    //返回的速度
    private float returnSpeed = 30f;

    void Awake()
    //有人提醒说rb的定义需要放在Awake中，而Start中会判空？
    {
        cd = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(canRotate)
        {
            //保证剑尖朝着剑的速度方向，更自然
            transform.right = rb.velocity;
        }

        //此时把剑加速传送回玩家手里，并在到达一定距离后销毁剑对象
        if(isReturning)
        {
            //Vector2.MoveTowards(向量起点, 向量终点, 移动速度)
            transform.position = Vector2.MoveTowards(transform.position, PlayerManager.instance.player.transform.position, returnSpeed * Time.deltaTime);
        
            //若剑与玩家剑的距离小于1，则销毁剑Prefab
            if(Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) < 1)
            {
                PlayerManager.instance.player.ClearAssignedSword();
            }
        }
    }

    public void ReturnTheSword()
    //决定是否把剑返回给玩家
    {
        //固定住剑的位置，不然的话剑得碰到地板或者敌人才能被召唤回来
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //使得剑Prefab变成无父对象的状态
        transform.parent = null;
        //设置可以返回
        isReturning = true;
        
        //rb.isKinematic = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    //剑实体与任何物体碰撞时这个函数被调用
    {
        //关闭旋转许可
        canRotate = false;
        //关闭剑的碰撞
        cd.enabled = false;
        //约束剑实体，使其冻结xyz三轴的变化
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //Kinematic时只能通过更改刚体的速度属性来改变位置，不会受到其它物理效果的影响，刚体将由动画或脚本通过更改transform.position进行完全控制
        rb.isKinematic = true;

        //将这把剑变成被碰撞的物体的子对象
        transform.parent = collision.transform;

        //对Enemy各子类怪物造成伤害
        if(transform.parent.GetComponentInParent<Enemy>() != null)
        {
            //对怪物造成剑的技能伤害，形式为飞剑的额外伤害加上人物的物理伤害，所以暴击时只计算人物的本体暴击伤害，飞剑本身的伤害值不计入暴击伤害的计算
            int _swordExtraDamage = PlayerManager.instance.player.sts.swordExtraDamage.GetValue();
            int _totalSwordDamage = _swordExtraDamage + PlayerManager.instance.player.sts.GetFinalPhysicalDamage();
            transform.parent.GetComponentInParent<EnemyStats>().GetPhysicalDamagedBy(_totalSwordDamage);
        }
    }

    public void SetupSword(Vector2 _dir, float _gravity)
    //用于被调用而初始化剑的状态
    {
        rb.velocity = _dir;
        rb.gravityScale = _gravity;
    }
}
