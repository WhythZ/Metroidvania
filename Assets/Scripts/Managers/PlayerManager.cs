using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region AbilityType
public enum AbilityType
{
    canWallSlide,
    CanDash,
    CanDoubleJump,
    CanThrowSword,
    CanFireBall,
    CanIceBall
}
#endregion

public class PlayerManager : MonoBehaviour, ISavesManager
//这里继承的是MonoBehaviour，所以Update一直在刷新
{
    #region Components
    //可以通过PlayerManager.instance来访问这个对象内的成员，想引用player需要用PlayerManager.instance.player.position而不是PlayerManager.instance.position，他妈的别再搞错了！
    //这个类只能有一个对象，不然会出问题
    public static PlayerManager instance {  get; private set; }

    //可以通过PlayerManager.instance.player来从任意地方访问到player，而不需要用GameObject.Find("Player")这种方式
    //若要将这个player绑定到此Manager，需要在Unity内创建一个PlayerManager对象，将Player对象绑定到这个脚本（脚本绑定到Manager对象）的此成员变量上
    public Player player;
    #endregion

    #region SaveData
    [Header("Currency")]
    //玩家持有的货币
    public int currency;
    [Header("Ability")]
    //玩家是否能滑墙
    public bool ability_CanWallSlide;
    //玩家是否能冲刺
    public bool ability_CanDash;
    //玩家是否能二段跳
    public bool ability_CanDoubleJump;
    //玩家是否能投掷剑
    public bool ability_CanThrowSword;
    //玩家是否能扔火球
    public bool ability_CanFireBall;
    //玩家是否能扔冰球
    public bool ability_CanIceBall;
    #endregion

    private void Awake()
    {
        //确保只有一个instance在工作，防止出问题
        if (instance != null)
        {
            //删除这个多出来的脚本
            //Destroy(instance);
            //直接删掉这个多余脚本所在的对象
            Destroy(instance.gameObject);
            Debug.Log("Invalid GameObject Containing PlayerManager's Instance DESTROYED");
        }
        else
            instance = this;
    }

    #region Ability
    public void ActivateAbility(AbilityType _type)
    //激活能力权限
    {
        if (_type == AbilityType.canWallSlide) { ability_CanWallSlide = true; }
        if (_type == AbilityType.CanDash) { ability_CanDash = true; }
        if (_type == AbilityType.CanDoubleJump) {  ability_CanDoubleJump = true; }
        if (_type == AbilityType.CanThrowSword) {  ability_CanThrowSword = true; }
        if (_type == AbilityType.CanFireBall) {  ability_CanFireBall = true; }
        if (_type == AbilityType.CanIceBall) {  ability_CanIceBall = true; }
    }
    public void DeactivateAbility(AbilityType _type)
    //关闭能力权限
    {
        if (_type == AbilityType.canWallSlide) { ability_CanWallSlide = false; }
        if (_type == AbilityType.CanDash) { ability_CanDash = false; }
        if (_type == AbilityType.CanDoubleJump) { ability_CanDoubleJump = false; }
        if (_type == AbilityType.CanThrowSword) { ability_CanThrowSword = false; }
        if (_type == AbilityType.CanFireBall) { ability_CanFireBall = false; }
        if (_type == AbilityType.CanIceBall) { ability_CanIceBall = false; }
    }
    #endregion

    #region ISavesManager
    public void LoadData(GameData _data)
    //加载游戏时候执行的操作
    {
        //读取货币数量
        this.currency = _data.currency;
        //读取能力许可
        ability_CanWallSlide = _data.canWallSlide;
        ability_CanDash = _data.canDash;
        ability_CanDoubleJump = _data.canDoubleJump;
        ability_CanThrowSword = _data.canThrowSword;
        ability_CanFireBall = _data.canFireBall;
        ability_CanIceBall = _data.canIceBall;
    }

    public void SaveData(ref GameData _data)
    {
        //存储货币数量
        _data.currency = this.currency;
        //存储能力许可
        _data.canWallSlide = ability_CanWallSlide;
        _data.canDash = ability_CanDash;
        _data.canDoubleJump = ability_CanDoubleJump;
        _data.canThrowSword = ability_CanThrowSword;
        _data.canFireBall = ability_CanFireBall;
        _data.canIceBall = ability_CanIceBall;
    }
    #endregion
}