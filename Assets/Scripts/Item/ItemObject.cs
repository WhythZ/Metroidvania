using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    //绑定自己的信息
    [SerializeField] ItemData itemData;

    private void OnValidate()
    //此函数在Unity的Hierarchy内进行各种对象的操作时，就会进行调用，而不用等到开始测试游戏时才进行更新（即在Start函数内更新）
    {
        //链接到SpriteRenderer，并赋予其图像为itemData内存储的icon，这样就可以在我们给这个ItemObject赋值了ItemData时立即更新图像，而不用等到Start后才能看到
        GetComponent<SpriteRenderer>().sprite = itemData.itemIcon;
        //同理，将ItemData中的物品名字赋予此GameObject
        gameObject.name = "ItemObject  " + itemData.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    //判断主角是否与物品发生了碰撞，记得保证物品Object有Collider组件
    {
        //若主角与物品的碰撞箱碰撞，且背包有余位，则捡起物品
        if(collision.GetComponent<Player>() != null && Inventory.instance.CanAddNewItem())
        {
            //通过instance调用物品栏，直接调用Inventory的instance即可
            //不同于PlayerManager的通过instance.player来调用，因为在那里instance代表的是PlayerManager而非Player类型对象
            Inventory.instance.AddItem(itemData);
            //销毁此item
            Destroy(gameObject);
        }
    }
}
