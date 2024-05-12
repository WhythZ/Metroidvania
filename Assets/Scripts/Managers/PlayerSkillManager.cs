using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
//这里继承的是MonoBehaviour，所以Update一直在刷新
{
    public static PlayerSkillManager instance {  get; private set; }
    //这里就不需要链接Player对象了，因为可以直接用PlayerManager来调用相关需求

    #region Skills
    public DashSkill dashSkill;
    public SwordSkill swordSkill;
    #endregion

    private void Awake()
    {
        //确保只有一个instance在工作，防止出问题
        if (instance != null)
        {
            //直接删掉这个多余脚本所在的对象
            Destroy(instance.gameObject);
            Debug.Log("Invalid PlayerManager Instance DESTROYED");
        }
        else
            instance = this;
    }

    private void Start()
    {
        #region Skills
        //链接这个dash到DashSkill的脚本;以便从任何地方通过此Manager访问Player的DashSkill
        dashSkill = GetComponent<DashSkill>();
        swordSkill = GetComponent<SwordSkill>();
        #endregion
    }
}
