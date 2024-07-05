using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlackholeSkill : PlayerSkill
{
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [Space]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public void UseSkill(Vector3 _position)
    {
        GameObject newBlackhole = Instantiate(blackholePrefab, _position, Quaternion.identity);

        BlackholeSkill_Controller newBlackholeScript = newBlackhole.GetComponent<BlackholeSkill_Controller>();

        newBlackholeScript.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown);
    }

    protected override void Update()
    {
        base.Update();
    }
}
