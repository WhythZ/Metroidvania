using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BlackholeHotKey : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;
    private Transform myEnemy;
    private BlackholeSkill_Controller blackhole;

    public void SetupHotKey(KeyCode _myhotKey, Transform _myEnemy, BlackholeSkill_Controller _myBlackhole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackhole = _myBlackhole;

        myHotKey = _myhotKey;
        myText.text = myHotKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackhole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
