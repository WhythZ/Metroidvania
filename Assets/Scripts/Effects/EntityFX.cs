using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    //链接到实体的Animator内的渲染器Component
    private SpriteRenderer sr;

    [Header("FlashFX")]
    //记录初始的材质
    private Material originMat;
    //记录用于受攻击动画效果的的材质
    [SerializeField] private Material flashHitMat;
    //记录用于受法术攻击动画效果的材质
    [SerializeField] private Material magicalHitMat;
    //材质更替后的停留时间
    [SerializeField] private float changeMatDuration = 0.1f;

    private void Start()
    {
        //链接到实体的Animator内的渲染器Component
        sr = GetComponentInChildren<SpriteRenderer>();

        //记录原始材质
        originMat = sr.material;
    }

    private IEnumerator FlashHitFX()
    //这个函数需要使用如fx.StartCoroutine("FlashHitFX");来调用，而不是直接用fx.FlashHitFX()
    {
        //使用受击材质
        sr.material = flashHitMat;
        //延迟一段时间
        yield return new WaitForSeconds(changeMatDuration);
        //回归原来的材质
        sr.material = originMat;
    }

    private IEnumerator MagicalHitFX()
    //这个函数需要使用如fx.StartCoroutine("MagicalHitFX");来调用，而不是直接用fx.MagicalHitFX()
    {
        //使用受击材质
        sr.material = magicalHitMat;
        //延迟一段时间
        yield return new WaitForSeconds(changeMatDuration);
        //回归原来的材质
        sr.material = originMat;
    }

    private void RedBlink()
    //调用方法示例bringer.fx.InvokeRepeating("RedBlink", 0, 0.1f);此为延迟零秒后以0.1f的频率持续调用
    {
        //此效果函数用于实体被弹反眩晕后处于的状态中进行红色的闪烁；在实体的StunnedState中被InvokeRepeating不断调用
        if(sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelRedBlink()
    //调用示例bringer.fx.Invoke("CancelRedBlink", 0);此为延迟零秒后调用此函数
    {
        //此函数用于取消MonoBehaviour中的所有InvokeRepeating，包括上面那个被Invoke的RedBlink函数
        CancelInvoke();
        //并确保人物颜色恢复为白色
        sr.color= Color.white;
    }
}
