using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    //防止Enter函数无限次数调用
    int xxx = 1;

    public override void Enter()
    //注意此处，由于没有转换出去死亡状态的条件，所以Exit一直不会触发，而Enter会被反复调用
    {
        base.Enter();

        if(xxx == 1)
        {
            //停止bgm
            AudioManager.instance.isPlayBGM = false;
            //死亡音效
            AudioManager.instance.PlaySFX(10, null);

            //触发死亡文字
            UI_MainScene.instance.PlayDeathText();

            //防止Enter函数无限次数调用
            xxx++;
        }

        //死亡触发屏幕的渐出；不知为啥这个如果放在上面那里面，则黑屏会FadeOut变黑后后重新变透明
        UI_MainScene.instance.fadeScreen.GetComponent<UI_FadeScreen>().FadeOut();
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
