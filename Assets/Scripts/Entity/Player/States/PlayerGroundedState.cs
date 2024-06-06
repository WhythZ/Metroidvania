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

        #region BasicInput
        //在地面的状态时（包括Idle和Move），若按空格且在地面上时，则进入跳跃状态
        if (Input.GetKeyDown(KeyCode.Space) && player.isGround)
        {
            //在主要UI显示的时候，不能进行此运动
            if (UI_MainScene.instance.ActivatedStateOfMainUIs() == true)
                return;

            stateMachine.ChangeState(player.jumpState);
        }
        //在地面上按下J键或者鼠标左键进入攻击状态
        if((Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Mouse0)) && player.isGround)
        {
            //在主要UI显示的时候，不能进行此运动
            if (UI_MainScene.instance.ActivatedStateOfMainUIs() == true)
                return;

            player.stateMachine.ChangeState(player.primaryAttackState);
        }
        //在地面上按下K键或鼠标右键进入防御反击状态
        if((Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Mouse1)) && player.isGround)
        {
            //在主要UI显示的时候，不能进行此运动
            if (UI_MainScene.instance.ActivatedStateOfMainUIs() == true)
                return;

            player.stateMachine.ChangeState(player.counterAttackState);
        }
        #endregion

        #region SkillInput
        //在地面上按下鼠标中键进入瞄准状态；前提是可以投掷，即比如可投掷剑数为1，则手中的剑没丢出去过才可以投掷
        if (Input.GetKeyDown(KeyCode.Mouse2) && player.isGround && !PlayerSkillManager.instance.assignedSword)
        {
            //能力限制
            if (PlayerManager.instance.ability_CanThrowSword == false)
                return;

            //在主要UI显示的时候，不能进行此运动
            if (UI_MainScene.instance.ActivatedStateOfMainUIs() == true)
                return;

            player.stateMachine.ChangeState(player.aimSwordState);
        }
        //如果玩家投掷出去剑了后，再按一次中键，则使剑返回到玩家手里
        if (PlayerSkillManager.instance.assignedSword && Input.GetKeyDown(KeyCode.Mouse2))
        {
            //在主要UI显示的时候，不能进行此运动
            if (UI_MainScene.instance.ActivatedStateOfMainUIs() == true)
                return;

            //调用玩家丢出去的剑对象的返回函数
            PlayerSkillManager.instance.assignedSword.GetComponent<Sword_Controller>().ReturnTheSword();
        }
        //火球发射
        if (Input.GetKeyDown(KeyCode.Alpha1) && player.isGround)
        {
            //能力限制
            if (PlayerManager.instance.ability_CanFireBall == false)
                return;

            //冷却时不可用，应当先检测不可用，不然放在后面的话，刚检测完如果可用，那么一定会被检测出不可用
            if (!PlayerSkillManager.instance.fireballSkill.CanUseSkill())
            {
                //调用文字弹出效果，提示技能处于冷却
                PlayerManager.instance.player.fx.CreatPopUpText("Cooldown", Color.white);
            }
            if (PlayerSkillManager.instance.fireballSkill.CanUseSkill() && !PlayerSkillManager.instance.assignedFireBall)
            {
                //在主要UI显示的时候，不能进行此运动
                if (UI_MainScene.instance.ActivatedStateOfMainUIs() == true)
                    return;

                //在玩家位置，朝向玩家面对方向生成球
                PlayerSkillManager.instance.fireballSkill.CreateFireBall(player.transform.position, player.facingDir);
            }
        }
        //冰球发射
        if (Input.GetKeyDown(KeyCode.Alpha2) && player.isGround)
        {
            //能力限制
            if (PlayerManager.instance.ability_CanIceBall == false)
                return;

            //冷却时不可用，应当先检测不可用，不然放在后面的话，刚检测完如果可用，那么一定会被检测出不可用
            if (!PlayerSkillManager.instance.iceballSkill.CanUseSkill())
            {
                //调用文字弹出效果，提示技能处于冷却
                PlayerManager.instance.player.fx.CreatPopUpText("Cooldown", Color.white);
            }
            if (PlayerSkillManager.instance.iceballSkill.CanUseSkill() && !PlayerSkillManager.instance.assignedIceBall)
            {
                //在主要UI显示的时候，不能进行此运动
                if (UI_MainScene.instance.ActivatedStateOfMainUIs() == true)
                    return;

                //在玩家位置，朝向玩家面对方向生成球
                PlayerSkillManager.instance.iceballSkill.CreateIceBall(player.transform.position, player.facingDir);
            }
        }
        #endregion
    }
}
