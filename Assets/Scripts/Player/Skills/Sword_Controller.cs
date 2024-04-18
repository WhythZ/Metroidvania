using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Controller : MonoBehaviour
{
    private BoxCollider2D cd;
    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    //有人提醒说rb的定义需要放在Awake中，而Start中会判空？
    {
        cd = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    public void SetupSword(Vector2 _dir, float _gravity)
    //用于被调用而初始化剑的状态
    {
        rb.velocity = _dir;
        rb.gravityScale = _gravity;
    }
}
