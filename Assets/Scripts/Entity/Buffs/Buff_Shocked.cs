using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使得该类可被检测到
[System.Serializable]
public class Buff_Shocked : Buff
{
    //眩晕状态下防御各项属性减少的百分比
    public float defenceDecreasePercentage = 0.2f;
}
