using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
//这里继承的是MonoBehaviour，所以Update一直在刷新
{
    public static PlayerSkillManager instance {  get; private set; }
    //这里就不需要链接Player对象了，因为可以直接用PlayerManager来调用相关需求

    #region Skills
    [Header("Skill List")]
    public DashSkill dashSkill;
    public SwordSkill swordSkill;
    public FireBallSkill fireballSkill;
    public IceBallSkill iceballSkill;
    public CloneSkill cloneSkill;
    public BlackholeSkill blackholeSkill;
    #endregion

    #region SkillObject
    [Header("Skill Object")]
    //记录是否已经投掷出去了剑，防止无限投掷，在GroundedState中（即投掷能力的入口处）检测是否已经创建过剑Prefab
    //那里的player.assignedSword可以当bool值使用
    public GameObject assignedSword;
    public GameObject assignedFireBall;
    public GameObject assignedIceBall;
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
        fireballSkill = GetComponent<FireBallSkill>();
        iceballSkill = GetComponent<IceBallSkill>();
        cloneSkill = GetComponent<CloneSkill>();
        blackholeSkill = GetComponent<BlackholeSkill>();
        #endregion
    }

    #region ObjectLife
    public void AssignNewSword(GameObject _newObject)
    {
        //记录一下新建了一个剑Prefab，在CreateSword()函数中被调用一次
        assignedSword = _newObject;
    }
    public void AssignNewFireBall(GameObject _newObject)
    {
        assignedFireBall = _newObject;
    }
    public void AssignNewIceBall(GameObject _newObject)
    {
        assignedIceBall = _newObject;
    }
    public void ClearAssignedSword()
    {
        //销毁多余的剑Prefab
        Destroy(assignedSword);
    }
    public void ClearAssignedFireBall()
    {
        Destroy(assignedFireBall);
    }
    public void ClearAssignedIceBall()
    {
        Destroy(assignedIceBall);
    }
    #endregion
}
