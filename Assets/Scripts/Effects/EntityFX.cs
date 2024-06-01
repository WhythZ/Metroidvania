using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    //链接到实体的Animator内的渲染器Component
    private SpriteRenderer sr;

    [Header("PopUpText")]
    //决定哪些实体会有这个效果，答案是玩家
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("Flash")]
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

    #region PopUpText
    public void CreatPopUpText(string _text, Color _color)
    //控制弹出这个文字效果的函数，接收需要弹出的内容及其颜色
    {
        //调整文字效果相对召唤者的生成位置，在范围内随机
        float _randomX = Random.Range(-1, 1);
        float _randomY = Random.Range(0.4f, 1f);
        Vector3 _positionOffset = new Vector3(_randomX, _randomY, 0);

        //调用预制体
        GameObject _newText = Instantiate(popUpTextPrefab, transform.position + _positionOffset, Quaternion.identity);

        _newText.GetComponent<TextMeshPro>().color = _color;
        _newText.GetComponent<TextMeshPro>().text = _text;
    }
    #endregion

    #region Attack
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
    #endregion
}
