using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //死亡触发屏幕的渐出
        UI.instance.fadeScreen.GetComponent<UI_FadeScreen>().FadeOut();

        //触发死亡文字
        UI.instance.PlayDeathText();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //死了就不能动了
        player.SetVelocity(0, 0);
    }
}
