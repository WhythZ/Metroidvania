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

    //一个int数组，记录该Stat在不同情况下的不同数值
    public List<int> modifiers;

    #region GetValue
    public int GetValue()
    //对外提供接口，使可以获取最终输出的数值
    {
        //finalValue是本Stat最终变成的值，在没有任何加成的情况下默认为baseValue
        int finalValue = baseValue;

        //比如如果装备了一个武器，这个武器对伤害的加成/削弱的数值会放在modifiers数组中
        foreach (int modifier in modifiers)
        {
            //添加所有加成/削弱得到最终伤害
            finalValue += modifier;
        }

        return finalValue;
    }
    #endregion

    #region SetValue
    public void SetValue(int _value)
    {
        baseValue = _value;
    }
    /*public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }*/
    #endregion

    #region EditModifiers
    public void AddModifier(int _modifier)
    {
        //添加元素
        modifiers.Add(_modifier);
    }
    public void RemoveModifier(int _modifier)
    {
        //删除元素
        modifiers.Remove(_modifier);
    }
    #endregion
}
