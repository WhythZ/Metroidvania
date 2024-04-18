using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //进入该状态时打开辅助瞄准轨迹点线；当剑被创建的时候设置为false，而不是在此状态结束时，因为剑出来的时间点是在throwSwordState的中间某时间点
        player.skill.swordSkill.ActivateDots(true);

        //瞄准时静止
        player.SetVelocity(0, 0);

        //如果鼠标瞄准的方向与角色面朝方向相反，则转身
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > player.transform.position.x)
        {
            if (player.facingDir == -1)
                player.Flip();
        }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < player.transform.position.x)
        {
            if (player.facingDir == 1)
                player.Flip();
        }

        //松开鼠标中键则离开此状态，进入投掷状态
        if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            player.stateMachine.ChangeState(player.throwSwordState);
        }
    }
}
