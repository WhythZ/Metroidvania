using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBallSkill : PlayerSkill
{
    #region IceBallSkillInfo
    [Header("IceBall Skill Info")]
    //Prefab的创建
    [SerializeField] private GameObject iceballPrefab;
    //向前方移动的速度
    public float moveSpeed = 12f;
    #endregion

    public void CreateIceBall(Vector3 _position, int _dir)
    {
        //生成冰球
        GameObject _newBall = Instantiate(iceballPrefab, _position, transform.rotation);
        //刷新冷却
        RefreshCooldown();

        //链接到控制器
        IceBall_Controller _control = _newBall.GetComponent<IceBall_Controller>();
        //初始化发射方向
        _control.SetupIceBall(_dir);
        //记录一下，防止玩家可以无限生成技能体
        PlayerSkillManager.instance.AssignNewIceBall(_newBall);

        //施法音效
        AudioManager.instance.PlaySFX(14, null);
    }
}
