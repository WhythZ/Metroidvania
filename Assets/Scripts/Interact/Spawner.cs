using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Spawner
    [Header("Spawner Settings")]
    //生成怪物的火苗效果
    [SerializeField] private ParticleSystem spawnerFX;
    //是否随机生成怪物
    [SerializeField] private bool randomSpawn = true;
    //如果不随机，则生成什么类型怪物
    [SerializeField] private EnemyType spawnType;
    //生成怪物的冷却
    [SerializeField] private float spawnCooldown = 15f;
    //一次性生成的数量
    [SerializeField] private int spawnAmount = 1;

    //是否进入了刷怪笼范围
    private bool isEnterSpawner;
    //是否可以继续生成怪物
    private bool canSpawn;

    //生成怪物的上限
    //[SerializeField] private int amountLimit;
    //记录生成的怪物
    //private GameObject[] spawnedEnemyList;
    #endregion

    private void Start()
    {
        canSpawn = true;
    }

    private void Update()
    {
        //刷怪控制
        SpawnController();

        //怪物生成数量限制
        //SpawnedEnemyAmountLimiter();
    }

    #region OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    //进入碰撞箱（即触发刷怪的区域）会触发随机刷怪
    {
        //必须是玩家，而非别的什么怪物都能触发
        if (collision.GetComponent<Player>() != null)
        {
            //代表进入了刷怪范围内
            isEnterSpawner = true;

            //开启刷怪的粒子效果
            spawnerFX.gameObject.SetActive(true);
            spawnerFX.Play();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            //代表离开了刷怪范围
            isEnterSpawner = false;

            //关闭效果
            spawnerFX.gameObject.SetActive(false);
            spawnerFX.Stop();
        }
    }
    #endregion

    #region Spawn
    public void SpawnController()
    {
        //进入刷怪笼范围，且刷怪冷却结束时可以刷怪
        if(isEnterSpawner && canSpawn)
        {
            if (randomSpawn)
            {
                SpawnRandomEnemy();
            }
            else
            {
                SpawnEnemy();
            }
            //先暂停可以生成，然后10s后恢复，防止一直被Update调用，无限生成
            canSpawn = false;
            Invoke("ReturnToCanSpawn", spawnCooldown);
        }
    }
    public bool ReturnToCanSpawn() => canSpawn = true;
    public void SpawnEnemy() => EnemyManager.instance.SpawnEnemy(spawnType, this.transform.position, spawnAmount);
    public void SpawnRandomEnemy() => EnemyManager.instance.SpawnRandomEnemy(this.transform.position, spawnAmount);
    #endregion

    #region AmountLimiter
    /*private void SpawnedEnemyAmountLimiter()
    {
        if(spawnedEnemyList.Length >= amountLimit)
        {
            canSpawn = false;
        }
    }*/
    #endregion
}
