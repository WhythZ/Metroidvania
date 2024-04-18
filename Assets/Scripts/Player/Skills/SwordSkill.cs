using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkill : PlayerSkill
{
    #region SwordSkillInfo
    [Header("Sword Skill Info")]
    //剑的Prefab的创建
    [SerializeField] private GameObject swordPrefab;
    //剑的重力加速度倍数
    [SerializeField] private float swordGravity;
    //剑的发射速度大小的控制向量
    [SerializeField] private Vector2 launchForce;
    //实时更新的剑的投掷瞄准方向，由于AimDirection()所得向量后续会被取归一化，坐标值很小，故需要每项乘上launchDir各项值
    private Vector2 finalAimDir;
    #endregion

    #region AimDirectionDots
    [Header("AimDirection Dots")]
    //瞄准辅助线的点的个数
    [SerializeField] private int dotsNum = 20;
    //点之间的间隔
    [SerializeField] private float spaceBetweenDots = 0.07f;
    //点的游戏内对象
    [SerializeField] private GameObject dotPrefab;
    //储存这些点的父对象的位置，创建在Unity内的Player身上，是一个Empty，即代表着控制所有点的位置的东西
    [SerializeField] private Transform dotsParent;
    //储存点的数组
    private GameObject[] dotsArray;
    #endregion

    protected void Start()
    {
        //生成一下轨迹需要用到的点
        GenerateDots();
    }

    protected override void Update()
    {
        base.Update();

        //注意这是GetKey而不是GetKeyDown，否则不能实时更新finalAimDir
        //实时更新的剑的投掷瞄准方向，注意这里的实现，可以单独控制向量的两个项
        //normalized归一化，表示取这个方向向量（一条射线，无限延伸，需要取一个代表性的点的值来代表这个方向）离向量原点较近的一个点与该远点组成向量代表此方向
        if (Input.GetKey(KeyCode.Mouse2))
            finalAimDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        //把点安插在抛物线上不同时间点（以(i * spaceBetweenDots)表示时间t）的等距位置
        if (Input.GetKey(KeyCode.Mouse2))
        {
            for (int i = 0; i < dotsNum; i++)
            {
                dotsArray[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    #region CreateSword
    public void CreateSword()
    //初始化剑的函数，设置为在PlayerAnimationTriggers脚本内的一个激发函数，插入playerThrowsSword动画的某帧触发
    //由于下列都是剑临时体临时使用的变量对象，故在这里创建局部变量
    {
        //初始化剑的Unity内对象、位置、旋转
        GameObject newSword = Instantiate(swordPrefab, PlayerManager.instance.player.transform.position, transform.rotation);
        //链接到剑的控制器
        Sword_Controller newSwordController = newSword.GetComponent<Sword_Controller>();
        //初始化剑的发射方向、刚体重力
        newSwordController.SetupSword(finalAimDir, swordGravity);

        //创建了剑出来之后，关闭辅助瞄准轨迹点线
        ActivateDots(false);
    }
    #endregion

    #region AimDirection
    public Vector2 AimDirection()
    //投掷前的瞄准方向
    {
        //记录玩家位置向量
        Vector2 playerPos = PlayerManager.instance.player.transform.position;
        //记录鼠标位置向量
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //方向向量是为从玩家指向鼠标这个方向
        Vector2 aimDirection = mousePos - playerPos;
        Debug.Log("Doing Aim Direction Detect");
        return aimDirection;
    }
    #endregion

    #region AimDirectionDots
    private void GenerateDots()
    //用于生成瞄准时预测的投掷轨迹，投掷出去后点需要消失
    {
        //先确定数组长度
        dotsArray = new GameObject[dotsNum];
        //给数组元素赋值，使其都对应上一个Unity内的Prefab对象
        for (int dot = 0; dot < dotsNum; dot++)
        {
            //Quaternion.identity就是指Quaternion(0,0,0,0)，就是每旋转前的初始角度,是一个确切的值类型，而transform.rotation是指本物体的角度，是值不确定的属性变量
            //Instantiate(Unity内对象预制体, 初始化位置, 旋转(此处表示不旋转), 对象的父对象);
            dotsArray[dot] = Instantiate(dotPrefab, PlayerManager.instance.player.transform.position, Quaternion.identity, dotsParent);
            
            //在赋值完之后立刻把这个对象关掉，因为我们需要的是ActivateDots函数来控制点的可见与否，本函数只是把对象塞到数组里而已
            dotsArray[dot].SetActive(false);
        }
    }
    public void ActivateDots(bool _isActivate)
    //控制轨迹点的可见与否
    {
        for (int i = 0; i < dotsNum; i++)
        {
            dotsArray[i].SetActive(_isActivate);
        }
    }
    private Vector2 DotsPosition(float t)
    //该把这些点放置在什么位置呢？即轨迹的抛物线上
    {
        //返回的位置向量由两个向量相加而成，第一个为玩家位置，第二个为finalAimDir向量随时间变化受重力影响后的状态值
        //finalAimDir向量即初速度v，(Physics2D.gravity * swordGravity)即重力加速度a，这个向量即为物理公式末速度v'=v*t+1/2*a*t^2
        //真正起到下坠模拟重力效果的是真末速度v'的y方向分速度，而此处把x分量也乘上了，无所谓；而Physics2D.gravity默认值为(0, -9.8)
        Vector2 pos = (Vector2)PlayerManager.instance.player.transform.position + finalAimDir * t + 0.5f * (t * t) * (Physics2D.gravity * swordGravity);
        
        return pos;
    }
    #endregion
}
