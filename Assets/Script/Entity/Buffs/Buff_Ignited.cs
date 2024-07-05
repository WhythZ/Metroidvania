using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使得该类可被检测到
[System.Serializable]
public class Buff_Ignited : Buff
{
    //处于灼烧状态时，每隔多长时间受到一次灼烧伤害
    public float damageCooldown = 1f;
    //每次受到灼烧伤害掉百分之多少血量
    public float burnHealthPercentage = 0.03f;
}
