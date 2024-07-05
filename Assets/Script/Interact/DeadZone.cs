using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
//当玩家落入悬崖时，需要触发死亡，不然会一直掉落
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EntityStats>() != null)
        {
            //利用数值处死实体，而不是使用StatsDie处死，会触发黑屏异常bug，应该是实际死亡需要触发两个Die函数的原因，单纯StatsDie不完整
            collision.GetComponent<EntityStats>().GetPhysicalDamagedBy(999999);
        }
        else
        {
            //若不是生物实体，则直接让这东西消失，比如唱片机、剑投掷物这种
            Destroy(collision.gameObject);
        }
    }
}
