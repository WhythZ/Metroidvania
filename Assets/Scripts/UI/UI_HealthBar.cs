using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//注意这里多调用了一个UI相关的
using UnityEngine.UI;


public class UI_HealthBar : MonoBehaviour
{
    #region Components
    //链接到实体的脚本
    private Entity entity;
    //链接到UI对象的RectTransform（即UI使用的Transform）
    private RectTransform myTransform;
    //控制血条的滑块，与实体血量链接
    private Slider slider;
    //链接到实体的统计信息
    private EntityStats mySts;
    #endregion

    private void Start()
    {
        #region Components
        //链接到Entity类的各种子类脚本（如Player.cs和Bringer.cs）
        entity = GetComponentInParent<Entity>();
        //链接到该血条UI的Transform
        myTransform = GetComponent<RectTransform>();
        //获取血条UI的滑块
        slider = GetComponentInChildren<Slider>();
        //链接到实体的统计信息
        mySts = GetComponentInParent<EntityStats>();
        #endregion

        #region Events
        //使得实体调用此函数时，多调用一个FlipTheUI函数，此处即+=运算符的重载，实现函数的相加
        entity.onFlipped += FlipTheUI;
        //使得每次onHealthChanged事件发生（被调用）时，调用更新血条UI的函数
        mySts.onHealthChanged += UpdateHealthUI;
        #endregion
        
        //实体生成的时候，调用一下血条UI的更新
        UpdateHealthUI();

        //这里的Start函数必须要确保比初始化实体血量的Start函数后调用，否则UI会与实际血量不符合
        //若想调整调用顺序，可在Project Settings的Scripts Execution Order处修改
        //Debug.Log("HealthBar_UI Start() Func Called");
    }

    //出于节省系统性能考虑，最好选择不使用Update函数更新血量，而是使用事件，在每一次对生命值进行更新时（受到伤害），调用一次更新UI的函数
    //而且，继承自MonoState类的脚本调用Update函数多了容易混乱，最好只在实体脚本内Update（各状态的Update实际上由其所属实体的Update调用，也是这个道理））
    /*    private void Update()
        {
            //UpdateHealthUI();
        }*/

    private void UpdateHealthUI()
    //与onHealthChanged事件叠加调用，不断更新实体的当前血量，以便于与滑块链接
    {
        //滑块的最大值，即实体的最大血量
        slider.maxValue = mySts.originalMaxHealth.GetValue();
        //滑块的当前值，即实体的当前血量
        slider.value = mySts.currentHealth;
    }

    //当实体转向后，把血条UI再旋转一次，即让UI别旋转
    private void FlipTheUI()
    {
        //再旋转一次，与实体的旋转抵消，即不旋转
        myTransform.Rotate(0, 180, 0);
    }

    private void OnOfEventsDisable()
    //在不需要的时候，解除这些事件发生时的额外调用的函数
    {
        entity.onFlipped -= FlipTheUI;
        mySts.onHealthChanged -= UpdateHealthUI;
    }
}