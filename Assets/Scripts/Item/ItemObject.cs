using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    //绑定自己的信息
    [SerializeField] ItemData itemData;
    //链接到SpriteRenderer
    private SpriteRenderer sr;

    private void Start()
    {
        //链接到SpriteRenderer
        sr = GetComponent<SpriteRenderer>();
        //从itemData传递Sprite图像
        sr.sprite = itemData.itemIcon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    //判断主角是否与物品发生了碰撞
    {
        //若主角与物品的碰撞箱碰撞，则捡起物品
        if(collision.GetComponent<Player>() != null)
        {
            //通过instance调用物品栏，直接调用Inventory的instance即可
            //不同于PlayerManager的通过instance.player来调用，因为在那里instance代表的是PlayerManager而非Player类型对象
            Inventory.instance.AddItem(itemData);
            //销毁此item
            Destroy(gameObject);
        }
    }
}
