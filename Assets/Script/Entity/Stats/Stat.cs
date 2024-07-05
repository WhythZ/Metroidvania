using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使得该类可被检测到
[System.Serializable]
public class Stat
//无继承，这是一个单纯表示一种自定义数值的类
{
    //该数值的基础数值
    [SerializeField] private int baseValue = 0;

    #region GetValue
    public int GetValue()
    //对外提供接口，使可以获取最终输出的数值
    {
        //finalValue是本Stat最终变成的值，在没有任何加成的情况下默认为baseValue
        int _finalValue = baseValue;
        return _finalValue;
    }
    #endregion

    #region SetValue
    public void SetValue(int _value)
    {
        baseValue = _value;
    }
    #endregion
}
