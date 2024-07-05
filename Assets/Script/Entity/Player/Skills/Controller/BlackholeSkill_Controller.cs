using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BlackholeSkill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;

    [Header("Clone Attack Info")]
    [SerializeField] private int amountOfAttacks = 4;
    [SerializeField] private float cloneAttackCoolDown = .3f;
    private float cloneAttackTimer;
            
    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCoolDown = _cloneAttackCooldown;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (targets.Count <= 0)
            {
                canShrink = true;
                DestroyHotKey();
            }
            else
                ReleaseCloneAttack();
        }
        //防止对于dashState的误判
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            canShrink = true;
            DestroyHotKey();
        }

        CloneAttackLogic();

        //黑洞缓慢变大
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x <= 0)
            {
                PlayerManager.instance.player.ExitBlackholeAbility();
                //canShrink = true;
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        DestroyHotKey();
        cloneAttackReleased = true;
        canCreateHotKeys = false;
    }

    //对列表内所有敌人造成一定次数的攻击
    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased)
        {
            cloneAttackTimer = cloneAttackCoolDown;
            
            while (amountOfAttacks > 0)
            {
                int randomIndex = Random.Range(0, targets.Count);
                float xOffset;

                if (Random.Range(0, 100) <= 50)
                    xOffset = .5f;
                else
                    xOffset = -.5f;

                PlayerSkillManager.instance.cloneSkill.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
                amountOfAttacks--;
            }

            if (amountOfAttacks <= 0)
            {
                cloneAttackReleased = false;
                canShrink = true;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("No more keys to assign!");
            return;
        }

        if (!canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        BlackholeHotKey newHotKeyScript = newHotKey.GetComponent<BlackholeHotKey>();
        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    private void DestroyHotKey()
    {
        if (createdHotKey.Count <= 0)
            return;

        for (int i = 0; i< createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    public void AddEnemyToList(Transform _enemytransform) => targets.Add(_enemytransform);
}
