using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BossBringerBattleState : EnemyState
{
    private BossBringer bossBringer;

    public BossBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.bossBringer = _bossBringer;
    }

    public override void Enter()
    {
        base.Enter();

        //����״̬�����ٶ�Ϊ�㣬��ֹ�����˺���ظı䳯��
        bossBringer.SetVelocity(0, 0);

        //������δ��������ս������idle״̬
        stateTimer = bossBringer.quitBattleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        #region ApproachPlayer
        //�������ڹ���ı�������Ҫת��
        if (bossBringer.battleMoveDir != bossBringer.facingDir)
        {
            bossBringer.Flip();
        }
        //��һ����Ҫ�ߵ���ȫ������غϵ�λ�ã�Ԥ��һ��ռ䣬�����attackCheckRadius��0.65������
        if ((rb.position.x < (PlayerManager.instance.player.transform.position.x + bossBringer.attackCheckRadius * 0.65f)) && (rb.position.x > (PlayerManager.instance.player.transform.position.x - bossBringer.attackCheckRadius * 0.65f)))
        {
            //�ڷ�Χ�ڵ���ͣ����ת����վ��״̬����Ȼ��ֱ���ߵ��غ�
            bossBringer.SetVelocity(0, 0);
            //�����������Χ�ڵ�ʱ�򣬱���վ��

            bossBringer.stateMachine.ChangeState(bossBringer.idleState);
        }
        else
        {
            {
                //��ֹ����������ߵ�̫��
                if(!bossBringer.enterAttackRegion)
                {
                    //���賯����ҵ��ƶ��ٶ�
                    bossBringer.SetVelocity(bossBringer.moveSpeed * bossBringer.battleSpeedMultiplier * bossBringer.battleMoveDir, rb.velocity.y);
                }
                else
                {
                    //��ֹ��battleState��ԭ��̤��
                    bossBringer.stateMachine.ChangeState(bossBringer.idleState);
                }
            }
        }
        #endregion

        #region Attack
        //��������˾������һ���ľ��뷶Χ�ڣ����ҹ�����ȴ�������㷢������
        if (bossBringer.enterAttackRegion && bossBringer.canAttack)
        {
            bossBringer.stateMachine.ChangeState(bossBringer.attackState);
        }
        //��������ڿɹ����뾶�ڣ����������ײ�˺�
        //if(PlayerManager.instance.transform.position)
        #endregion

        #region CaseThatQuitBattle
        //����������£�����ս����idle
        if (!bossBringer.isGround)
        {
            //bossBringer.stateMachine.ChangeState(bossBringer.idleState);
            //��������Ҹ���
            Debug.Log("from battleState1");
            bossBringer.stateMachine.ChangeState(bossBringer.teleportState);
        }
        //��ŭʱ�䵽�˺󣬻�����Ҿ��볬����Χ������ս
        if (stateTimer < 0 || Vector2.Distance(bossBringer.transform.position, PlayerManager.instance.player.transform.position) > bossBringer.GetQuitBattleDisance()/* && bossBringer.teleportTimer < 0*/)
        {
            //�ر�����״̬
            //bossBringer.shouldEnterBattle = false;
            //�ص�վ��״̬
            //bossBringer.stateMachine.ChangeState(bossBringer.idleState);
            //��������Ҹ���
            Debug.Log("from battleState2");
            bossBringer.stateMachine.ChangeState(bossBringer.idleState);
        }
        #endregion
    }
}
