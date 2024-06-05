using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使得该类可被检测到
[System.Serializable]
public class Buff
{
    //该Buff的激活状态
    [SerializeField] protected bool status;
    //该Buff的持续时长
    public float duration = 5f;

    public virtual bool GetStatus() => status;

    public virtual void SetStatus(bool _bool) => status = _bool;
}
