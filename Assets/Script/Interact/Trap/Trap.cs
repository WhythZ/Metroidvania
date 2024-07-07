using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    //陷阱碰撞箱
    protected BoxCollider2D cd;
    //陷阱伤害大小
    [SerializeField] protected int trapDamage;

    virtual protected void Awake()
    {
        cd = GetComponent<BoxCollider2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //陷阱应当是所有实体都能触发的，包括玩家和敌人
        if (collision.GetComponent<Entity>() != null)
        {
            //陷阱触发的效果
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity>() != null)
        {
            //离开陷阱的效果
        }
    }
}
