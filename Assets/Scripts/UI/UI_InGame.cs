using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_InGame : MonoBehaviour
{
    private PlayerStats pStats;
    private UnityEngine.UI.Slider healthBarSlider;

    private void Start()
    {
        pStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        healthBarSlider = GetComponentInChildren<UnityEngine.UI.Slider>();

        //事件的调用
        if (pStats != null)
            pStats.onHealthChanged += UpdateHealthUI;

        //这里的Start函数必须要确保比初始化实体血量的Start函数后调用，否则UI会与实际血量不符合
        //若想调整调用顺序，可在Project Settings的Scripts Execution Order处修改
        //Debug.Log("UI_InGame Start() Func Called");
        //实体生成的时候，调用一下血条UI的更新
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    //与onHealthChanged事件叠加调用，不断更新实体的当前血量，以便于与滑块链接
    {
        //滑块的最大值，即实体最终最大血量
        healthBarSlider.maxValue = pStats.GetFinalMaxHealth();
        //滑块的当前值，即实体的当前血量
        healthBarSlider.value = pStats.currentHealth;
    }
}
