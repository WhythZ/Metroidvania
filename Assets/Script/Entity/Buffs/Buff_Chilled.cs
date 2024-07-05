using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使得该类可被检测到
[System.Serializable]
public class Buff_Chilled : Buff
{
    //处于冰冻状态时，所有速度减慢
    public float slowPercentage = 0.3f;
}
