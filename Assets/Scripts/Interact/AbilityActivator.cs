using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityActivator : MonoBehaviour
{
    //要激活的能力种类
    [SerializeField] private AbilityType abilityType;
    //粒子效果
    private ParticleSystem particle;

    private void Start()
    {
        if (GetComponentInChildren<ParticleSystem>() != null)
        {
            particle = GetComponentInChildren<ParticleSystem>();
            particle.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //玩家与之接触后触发能力激活
        if (collision.GetComponent<Player>() != null)
        {
            //激活对应能力
            PlayerManager.instance.ActivateAbility(abilityType);

            #region ActivationFX
            //激活能力的音效
            AudioManager.instance.PlaySFX(16, null);

            //激活能力的对应特效
            #endregion

            //关闭自身
            this.gameObject.SetActive(false);
        }
    }
}
