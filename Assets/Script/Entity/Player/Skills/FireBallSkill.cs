using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : PlayerSkill
{
    #region FireBallSkillInfo
    [Header("FireBall Skill Info")]
    //Prefab的创建
    [SerializeField] private GameObject fireballPrefab;
    //向前方移动的速度
    public float moveSpeed = 12f;
    #endregion

    public void CreateFireBall(Vector3 _position, int _dir)
    {
        //生成火球
        GameObject _newBall = Instantiate(fireballPrefab, _position, transform.rotation);
        //刷新冷却
        RefreshCooldown();

        //链接到控制器
        FireBall_Controller _control = _newBall.GetComponent<FireBall_Controller>();
        //初始化发射方向
        _control.SetupFireBall(_dir);
        //记录一下，防止玩家可以无限生成技能体
        PlayerSkillManager.instance.AssignNewFireBall(_newBall);

        //施法音效
        AudioManager.instance.PlaySFX(14, null);
    }
}
