using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Controller : MonoBehaviour
//此脚本在剑身上；为了防止剑粘到玩家或者其他物体身上，应该为剑单独设置一个图层Projectile，并在ProjectSettings的Physics2D内将其仅勾选Ground和Enemy
{
    #region Components
    private BoxCollider2D cd;
    private Rigidbody2D rb;
    private Animator anim;
    #endregion

    //是否处于返回状态，若是，则应当返回到玩家处
    private bool isReturning = false;

    /*[Header("Sword Bounce Info")]
    //当前可以弹的次数
    public int bounceNumber;
    //检测并储存范围内可弹的敌人目标位置
    public List<Transform> enemyTargets;
    //决定下一次往哪里弹，即上面那个列表的编号
    private int targetIndex = 0;*/

    private void Awake()
    //有人提醒说组件赋值需要放在Awake中，而Start中会判空？还真是！
    {
        #region Components
        cd = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        #endregion

        /*//初始化弹跳数量
        bounceNumber = PlayerSkillManager.instance.swordSkill.bounceMaxAmount;*/
    }

    private void Update()
    {
        //在别的什么东西身上的时候不准变向
        if (transform.parent == null)
        {
            //保证剑尖朝着剑的速度方向，更自然
            transform.right = rb.velocity;
        }

        #region Return
        //此时把剑加速传送回玩家手里，并在到达一定距离后销毁剑对象
        if (isReturning)
        {
            //Vector2.MoveTowards(向量起点, 向量终点, 移动速度)
            transform.position = Vector2.MoveTowards(transform.position, PlayerManager.instance.player.transform.position, PlayerSkillManager.instance.swordSkill.returnSpeed * Time.deltaTime);

            //若剑与玩家剑的距离小于1，则销毁剑Prefab
            if (Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) < 1)
            {
                PlayerSkillManager.instance.ClearAssignedSword();
            }
        }
        #endregion

        /*#region Bounce
        //可弹跳的基本条件：可弹跳次数大于零、有敌人可以被弹、不处于返程状态
        if (bounceNumber > 0 && enemyTargets.Count > 0 && !isReturning)
        {
            //防止数组越界
            if (targetIndex >= enemyTargets.Count)
            {
                //说明弹完了，那就自动返回
                ReturnTheSword();
                Debug.Log("Automatically Return");
                return;
            }

            //若是下一个敌人丢失（比如死了），那么继续锁定下一个敌人（因为要用到transform，所以如果是空的会报错）
            if (enemyTargets[targetIndex] != null)
            {
                Debug.Log("Move Towards " + targetIndex);
                //从当前位置以一定速度向下一个目标移动
                transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position, PlayerSkillManager.instance.swordSkill.bounceSpeed * Time.deltaTime);
            
                //当到达目标位置附近后，向下一个目标前进
                if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < 0.5f)
                {
                    Debug.Log("Reach Target " + targetIndex);
                    //定位下一个目标
                    targetIndex++;
                    //可弹跳数量减一
                    bounceNumber--;
                }
            }
        }
        #endregion*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    //剑实体与任何物体碰撞时这个函数被调用
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            #region Damage
            //对Enemy各子类怪物造成伤害
            //存储要用到的传入参数
            EntityStats _sts = PlayerManager.instance.player.sts;
            int _swordDamage = PlayerManager.instance.player.sts.swordDamage.GetValue();
            //造成技能伤害，仅物理伤害
            collision.transform.GetComponent<EnemyStats>().GetTotalSpecialDmgFrom(_sts, _swordDamage, true, true, false, false, true);
            #endregion

            /*#region Bounce
            //这个条件只有第一次碰撞的时候才会触发，即只会从第一个敌人的位置开始检测一次范围内敌人
            if (enemyTargets.Count == 0)
            {
                //获取检测半径内的敌人
                Collider2D[] _colliders = Physics2D.OverlapCircleAll(transform.position, PlayerSkillManager.instance.swordSkill.bounceRadius);

                //逐一获取这些敌人的位置
                foreach (var _target in _colliders)
                {
                    if (_target.GetComponent<Enemy>() != null)
                        enemyTargets.Add(_target.transform);
                }

                Debug.Log("Record " + enemyTargets.Count + " Enemies");
            }
            #endregion*/
        }

        //卡进去
        StuckInto(collision);
    }

    #region Return
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
    #endregion

    #region Stuck
    private void StuckInto(Collider2D _collision)
    {
        //关闭剑的碰撞
        cd.enabled = false;
        //约束剑实体，使其冻结xyz三轴的变化
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //Kinematic时只能通过更改刚体的速度属性来改变位置，不会受到其它物理效果的影响，刚体将由动画或脚本通过更改transform.position进行完全控制
        rb.isKinematic = true;
        //将这把剑变成被碰撞的物体的子对象
        transform.parent = _collision.transform;
    }
    #endregion

    #region Setup
    public void SetupSword(Vector2 _dir, float _gravity)
    //用于被调用而初始化剑的状态
    {
        rb.velocity = _dir;
        rb.gravityScale = _gravity;
    }
    #endregion
}
