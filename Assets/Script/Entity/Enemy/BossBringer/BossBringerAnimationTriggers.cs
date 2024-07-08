using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerAnimationTriggers : MonoBehaviour
{
    BossBringer bossBringer => GetComponentInParent<BossBringer>();

    private void AnimationTrigger()
    {
        bossBringer.AnimationTrigger();
    }

    private void ReLocate() => bossBringer.FindPosition();

    private void AttackDamageTrigger()
    {
        Collider2D[] collidersInAttackZone = Physics2D.OverlapCircleAll(bossBringer.attackCheck.position, bossBringer.attackCheckRadius);

        foreach (var beHitEntity in collidersInAttackZone)
        {
            if (beHitEntity.GetComponent<Player>() != null)
            {
                //�������ٶԷ�����ֵ�������ܻ�Ч��
                beHitEntity.GetComponent<PlayerStats>().GetTotalNormalDmgFrom(bossBringer.sts, true, true);
            }
        }
    }

    //�������Ա�����ѣ�ε�״̬
    private void OpenCounterAttackWindow() => bossBringer.OpenCounterAttackWindow();

    //�رտ��Ա�����ѣ�ε�״̬
    private void CloseCounterAttackWindow() => bossBringer.CloseCounterAttackWindow();

    //��������
    private void BossBringerDead()
    {
        //������ᷢ��������д����

        //����ʵ��
        Destroy(bossBringer.gameObject);
    }
}
