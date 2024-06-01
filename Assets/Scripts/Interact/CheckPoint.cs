using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator anim;

    //代表特定存档点的标识
    public string id;

    //记录这个存档点是否被激活过
    public bool isActive;

    //这个函数每次调用都会生成一个新的ID，所以只需要借助ContextMenu调用一次
    //在Unity内该脚本对象上右键该脚本组件，点击"Generate CheckPoint ID"即可调用此函数
    [ContextMenu("Generate CheckPoint ID")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            //接触火堆触发火堆的激活状态，表示重生点激活
            ActivateCheckPoint();
        }
    }

    public void ActivateCheckPoint()
    {
        //激活的音效
        //AudioManager.instance.PlaySFX()
        
        //记录激活的状态
        isActive = true;

        //激活的动画
        anim.SetBool("active", true);        
    }
}
