using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : PlayerSkill
//继承自PlayerSkill，每个PlayerSkill的子类脚本都要在Unity内拖入PlayerSkillManager内
{
    //我们在Unity内设置这个技能的cooldown即可，故而Player.cs脚本内不需要什么dashTimer或者dashCooldown了
    //直接通过PlayerSkillManager.instance.dash.cooldown等访问此脚本的相关信息即可
}