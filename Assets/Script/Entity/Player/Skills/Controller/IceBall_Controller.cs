using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall_Controller : MonoBehaviour
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
        Invoke("DestroySkillObject", PlayerSkillManager.instance.iceballSkill.cooldown);
    }

    private void Update()
    {
        //朝这个方向行驶
        rb.velocity = new Vector2(direction, 0) * PlayerSkillManager.instance.iceballSkill.moveSpeed;

        //保证贴图朝着速度方向
        transform.right = rb.velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //进入爆炸动画
        anim.SetBool("boom", true);

        if (collision.GetComponent<EnemyStats>() != null)
        {
            //存储要用到的传入参数
            EntityStats _sts = PlayerManager.instance.player.sts;
            int _skilldmg = PlayerManager.instance.player.sts.iceballDamage.GetValue();
            //造成技能伤害，仅法术伤害；造成冷冻buff
            collision.GetComponent<EnemyStats>().GetTotalSkillDmgFrom(_sts, _skilldmg, false, true, false, true, false);
        }
    }

    public void SetupIceBall(int _dir)
    //用于被调用而初始化状态
    {
        direction = _dir;
    }

    public void DestroySkillObject()
    //在Animator处爆炸完成后调用消除
    {
        PlayerSkillManager.instance.ClearAssignedIceBall();
    }
}
