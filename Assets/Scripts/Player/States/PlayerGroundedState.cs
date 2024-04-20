using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
//这是一个包含Idle和Move两种状态的大状态（super state）；这个状态是一种PlayerState（人物在地面时候的状态），故继承自PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //每次碰触地面时可跳跃次数恢复为2
        player.jumpNum = 2;

        //每次碰触地面或者墙壁后，则可以继续在冷却结束后进行冲刺
        player.CanDashSetting(true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //在地面的状态时（包括Idle和Move），若按空格且在地面上时，则进入跳跃状态
        if(Input.GetKeyDown(KeyCode.Space) && player.isGround)
            stateMachine.ChangeState(player.jumpState);

        //在地面上按下J键或者鼠标左键进入攻击状态
        if((Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Mouse0)) && player.isGround)
            player.stateMachine.ChangeState(player.primaryAttack);

        //在地面上按下K键或鼠标右键进入防御反击状态
        if((Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Mouse1)) && player.isGround)
            player.stateMachine.ChangeState(player.counterAttackState);

        #region SwordSkill
        //在地面上按下鼠标中键进入瞄准状态
        //前提是可以投掷，即比如可投掷剑数为1，则手中的剑没丢出去过才可以投掷
        if (Input.GetKeyDown(KeyCode.Mouse2) && player.isGround && !player.assignedSword)
            player.stateMachine.ChangeState(player.aimSwordState);

        //如果玩家投掷出去剑了后，再按一次中键，则使剑返回到玩家手里
        if (PlayerManager.instance.player.assignedSword && Input.GetKeyDown(KeyCode.Mouse2))
        {
            //调用玩家丢出去的剑对象的返回函数
            player.assignedSword.GetComponent<Sword_Controller>().ReturnTheSword();
        }
        #endregion
    }
}
