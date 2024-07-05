using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    bringer,
    slime_Big,
    slime_Medium,
    slime_Small
}

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Header("Enemy Prefab")]
    //使用列表记录怪物预制体
    [SerializeField] private GameObject[] enemyPrefabList;

    private void Awake()
    {
        //确保管理器仅有一个
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public void SpawnEnemy(EnemyType _enemy, Vector3 _position, int _count)
    {
        //生成指定个数
        for (int i = 0; i < _count; i++)
        {
            //随机化水平位置
            float _xPos = UnityEngine.Random.Range(-5f, 5f);

            //在选定的位置生成选定的怪物
            GameObject _newSlime = Instantiate(EnemyTypeMapping(_enemy), _position + new Vector3(_xPos, 0), Quaternion.identity);
        }
    }

    public void SpawnRandomEnemy(Vector3 _position, int _count)
    {
        //生成指定个数
        for (int i = 0; i < _count; i++)
        {
            //随机选取列表内的一个怪物
            int _random = UnityEngine.Random.Range(0, enemyPrefabList.Length);

            //生成怪物
            SpawnEnemy(enemyPrefabList[_random].GetComponent<Enemy>().enemyType, _position, _count);
        }
    }

    public GameObject EnemyTypeMapping(EnemyType _enemy)
    {
        if (_enemy == EnemyType.bringer) { return enemyPrefabList[0]; }
        if (_enemy == EnemyType.slime_Big) { return enemyPrefabList[1]; }
        if (_enemy == EnemyType.slime_Medium) { return enemyPrefabList[2]; }
        if (_enemy == EnemyType.slime_Small) { return enemyPrefabList[3]; }
        return null;
    }
}
