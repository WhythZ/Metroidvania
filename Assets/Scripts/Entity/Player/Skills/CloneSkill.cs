using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : MonoBehaviour
//这个技能是在某处出现玩家的分身进行攻击，目前用于黑洞技能结束后被调用
{
    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        //Debug.Log("Create Clone!");
        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset);
    }
}
