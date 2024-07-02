using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : MonoBehaviour
{
    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        Debug.Log("Create Clone!");
        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset);
    }
}
