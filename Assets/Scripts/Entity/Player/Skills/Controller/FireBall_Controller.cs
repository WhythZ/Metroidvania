using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall_Controller : MonoBehaviour
{
    #region Components
    private BoxCollider2D cd;
    private Rigidbody2D rb;
    private Animator anim;
    #endregion

    //前进方向
    private int direction;

    private void Awake()
    {
        #region Components
        cd = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        #endregion
    }

    private void Start()
    {
        //在技能cd结束后销毁
        Invoke("DestroySkillObject", PlayerSkillManager.instance.fireballSkill.cooldown);
    }

    private void Update()
    {
        //朝这个方向行驶
        rb.velocity = new Vector2(direction, 0) * PlayerSkillManager.instance.fireballSkill.moveSpeed;

        //保证贴图朝着速度方向
        transform.right = rb.velocity;
    }

    /*
     *问题：OnTriggerEnter2D(Collider2D collision)可以在Animator的动画内被调用，但是怎么解决这个传入参数的问题呢？
     *解决了的话我们就可以多次调用此函数，来造成爆炸的多段伤害的效果
     */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //进入爆炸动画
        anim.SetBool("boom", true);

        if (collision.GetComponent<EnemyStats>() != null)
        {
            //存储要用到的传入参数
            EntityStats _sts = PlayerManager.instance.player.sts;
            int _skilldmg = PlayerManager.instance.player.sts.fireballDamage.GetValue();
            //造成技能伤害，仅法术伤害；造成灼烧buff
            collision.GetComponent<EnemyStats>().GetTotalSpecialDmgFrom(_sts, _skilldmg, false, true, true, false, false);
        }
    }

    public void SetupFireBall(int _dir)
    //用于被调用而初始化状态
    {
        direction = _dir;
    }

    public void DestroySkillObject()
    //在Animator处爆炸完成后调用消除
    {
        PlayerSkillManager.instance.ClearAssignedFireBall();
    }
}
