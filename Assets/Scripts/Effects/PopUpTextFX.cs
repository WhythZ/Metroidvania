using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour
{
    public TextMeshPro myText;

    //文字效果出现的初速度
    [SerializeField] private float textAppearSpeed;
    //文字效果消失的速度
    [SerializeField] private float textDisappearSpeed;
    [SerializeField] private float colorDisappearSpeed;

    //文字效果存在的时间
    [SerializeField] private float lifeTime;
    //计时器
    private float textTimer;

    private void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }

    private void Update()
    {
        textTimer -= Time.time;
    
        if(textTimer < 0)
        {
            float _alpha = myText.color.a - colorDisappearSpeed * Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, _alpha);
        
            if(myText.color.a < 50)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), textDisappearSpeed * Time.deltaTime);
            }

            if (myText.color.a <= 0)
                Destroy(gameObject);
            }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), textAppearSpeed * Time.deltaTime);
        }
    }
}
