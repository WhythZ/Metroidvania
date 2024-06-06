using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_InGame : MonoBehaviour
{
    private PlayerStats pStats;
    private UnityEngine.UI.Slider healthBarSlider;

    [Header("Skill UI")]
    //手动在Hierarchy内赋值吧，Start获取到该组件比较难，因为Image类型的太多了
    [SerializeField] private UnityEngine.UI.Image dashCooldownImage;
    [SerializeField] private UnityEngine.UI.Image swordCooldownImage;
    [SerializeField] private UnityEngine.UI.Image fireballCooldownImage;
    [SerializeField] private UnityEngine.UI.Image iceballCooldownImage;

    private void Start()
    {
        pStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        healthBarSlider = GetComponentInChildren<UnityEngine.UI.Slider>();

        //血条更新事件的调用
        if (pStats != null)
            pStats.onHealthChanged += UpdateHealthUI;

        //这里的Start函数必须要确保比初始化实体血量的Start函数后调用，否则UI会与实际血量不符合
        //若想调整调用顺序，可在Project Settings的Scripts Execution Order处修改
        //Debug.Log("UI_InGame Start() Func Called");
        //实体生成的时候，调用一下血条UI的更新
        UpdateHealthUI();
    }

    private void Update()
    //由于游戏内UI需要实时检测一些重要功能，如各UI切换，技能冷却显示UI等，所以对这些功能使用Update函数
    {
        UpdateSwordCooldown();
        UpdateDashCooldown();
        UpdateFireBallCooldown();
        UpdateIceBallCooldown();
    }

    #region HealthUI
    private void UpdateHealthUI()
    //与onHealthChanged事件叠加调用，不断更新实体的当前血量，以便于与滑块链接
    {
        //滑块的最大值，即实体最终最大血量
        healthBarSlider.maxValue = pStats.GetFinalMaxHealth();
        //滑块的当前值，即实体的当前血量
        healthBarSlider.value = pStats.currentHealth;
    }
    #endregion

    #region SkillCooldown
    private void UpdateSwordCooldown()
    {
        //剑被召唤出来时时使得技能进入冷却
        if (PlayerSkillManager.instance.assignedSword)
            ResetSkillCooldownUIFor(swordCooldownImage);

        //剑丢出去后一直处于冷却状态，除非收回剑
        if (!PlayerSkillManager.instance.assignedSword)
            swordCooldownImage.fillAmount = 0;
    }
    private void UpdateDashCooldown()
    {
        //dash技能冷却条的更新
        UpdateSkillCooldownUIOf(dashCooldownImage, PlayerSkillManager.instance.dashSkill.cooldown);

        //按下了左shift且玩家能进行冲刺（即玩家进行了冲刺）时，预示着玩家冲刺技能进入冷却，故而更新技能图标进入冷却
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //防止在按了shift而没有进行冲刺的情况下（如攻击时按shift是无法冲刺的）刷新冷却条UI
            if(PlayerManager.instance.player.stateMachine.currentState == PlayerManager.instance.player.dashState)
                ResetSkillCooldownUIFor(dashCooldownImage);
            else
            {
                //调用文字弹出效果，提示技能处于冷却
                PlayerManager.instance.player.fx.CreatPopUpText("Cooldown", Color.white);
            }
        }
    }
    private void UpdateFireBallCooldown()
    {
        //技能冷却条的更新
        UpdateSkillCooldownUIOf(fireballCooldownImage, PlayerSkillManager.instance.fireballSkill.cooldown);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResetSkillCooldownUIFor(fireballCooldownImage);
        }
    }
    private void UpdateIceBallCooldown()
    {
        //技能冷却条的更新
        UpdateSkillCooldownUIOf(iceballCooldownImage, PlayerSkillManager.instance.iceballSkill.cooldown);

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ResetSkillCooldownUIFor(iceballCooldownImage);
        }
    }
    #endregion

    #region SkillUI
    private void UpdateSkillCooldownUIOf(UnityEngine.UI.Image _image, float _cooldown)
    //当一个技能处于冷却时，对其冷却条进行递减，直到冷却结束
    {
        if(_image.fillAmount > 0)
        {
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
        }
    }
    private void ResetSkillCooldownUIFor(UnityEngine.UI.Image _image)
    //使得游戏中UI中的技能图标进行进入冷却的设置
    {
        //当调用此函数时，技能图标进入冷却状态
        //这里的约束条件使得即使在冷却未结束前再次按了触发冷却的键，也不会重新从头开始显示冷却时间，所以Update函数内的判断条件可以不用写的那么全
        if(_image.fillAmount <= 0)
        {
            _image.fillAmount = 1;
        }
    }
    #endregion
}
