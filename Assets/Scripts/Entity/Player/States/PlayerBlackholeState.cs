using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .5f;
    private bool skillUsed;
    private float defaultGravity;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillUsed = false;
        stateTimer = flyTime;
        defaultGravity = player.rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = defaultGravity;
    }

    public override void TriggerWhenAnimationFinished()
    {
        base.TriggerWhenAnimationFinished();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 5);

        if (stateTimer <= 0)
        {
            rb.velocity = new Vector2(0, -.1f);

            if (!skillUsed && player.skill.blackholeSkill.CanUseSkill())
            {
                skillUsed = true;
            }
        }
    }
}
