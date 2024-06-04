using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
//����̳е���MonoBehaviour������Updateһֱ��ˢ��
{
    //ÿ�����������ȴʱ��
    public float cooldown;
    //������ȴ�ļ�ʱ��
    protected float cooldownTimer;

    protected virtual void Update()
    {
        //��ʱ��ݼ���ÿ���1��λ
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool WhetherCanUseSkill()
    {
        if(cooldownTimer < 0)
        {
            //��������ȴ���ڿ��ý׶�ʱ��ʹ�ü���
            UseSkill();
            //�ָ���ȴʱ�䣬Ȼ�󷵻ؿ���ʹ�ü��ܵ��ź�true
            cooldownTimer = cooldown;
            
            return true;
        }
        else
        {
            //�������ֵ���Ч������ʾ���ܴ�����ȴ
            PlayerManager.instance.player.fx.CreatPopUpText("Cooldown", Color.white);

            return false;
        }
    }

    public virtual void UseSkill()
    {
        //ʹ�øü��ܲ�����Ч�������ڱ��̳к���д
    }
}