using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    //链接到实体的Animator内的渲染器Component
    private SpriteRenderer sr;

    #region PopUpText
    [Header("Pop Up Text")]
    //决定哪些实体会有这个效果，答案是玩家
    [SerializeField] private GameObject popUpTextPrefab;
    #endregion

    #region Attack
    [Header("Damaged Material")]
    //记录初始的材质
    private Material originMat;
    //记录用于受攻击动画效果的的材质
    [SerializeField] private Material flashHitMat;
    //记录用于受法术攻击动画效果的材质
    [SerializeField] private Material magicalHitMat;
    //材质更替后的停留时间
    [SerializeField] private float changeMatDuration = 0.1f;
    [Header("Hit FX")]
    [SerializeField] private GameObject hitFX00;
    [SerializeField] private GameObject hitFX01;
    #endregion

    #region Ailments
    [Header("Ailments Color")]
    [SerializeField] private Color ignitedColor;
    [SerializeField] private Color chilledColor;
    [SerializeField] private Color shockedColor;

    [Header("Ailments Particle")]
    [SerializeField] private ParticleSystem ignitedFX;
    [SerializeField] private ParticleSystem chilledFX;
    [SerializeField] private ParticleSystem shockedFX;
    #endregion

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
        float _randomX = Random.Range(-1.5f, 1.5f);
        float _randomY = Random.Range(0.5f, 2f);
        Vector3 _positionOffset = new Vector3(_randomX, _randomY, 0);

        //调用预制体
        GameObject _newText = Instantiate(popUpTextPrefab, transform.position + _positionOffset, Quaternion.identity);

        _newText.GetComponent<TextMeshPro>().color = _color;
        _newText.GetComponent<TextMeshPro>().text = _text;
    }
    #endregion

    #region Clear
    private void CancelColorChange()
    //调用示例bringer.fx.Invoke("CancelColorChange", 0);此为延迟零秒后调用此函数
    {
        //此函数用于取消MonoBehaviour中的所有InvokeRepeating，包括那个被Invoke的RedBlink函数
        CancelInvoke();
        //并确保人物颜色恢复为白色
        sr.color = Color.white;

        //取消所有粒子效果
        ignitedFX.gameObject.SetActive(false);
        chilledFX.gameObject.SetActive(false);
        shockedFX.gameObject.SetActive(false);
        ignitedFX.Stop();
        chilledFX.Stop();
        shockedFX.Stop();
    }
    #endregion

    #region HitFX
    public void CreatHitFX00(Transform _target)
    //传入敌人位置，受击效果产生在敌人身上
    {
        //随机的位移与旋转，使得效果看起来不一样
        float _xPosition = UnityEngine.Random.Range(-0.5f, 0.5f);
        float _yPosition = UnityEngine.Random.Range(-0.5f, 0.5f);
        //float _zRotation = UnityEngine.Random.Range(-90, 90);

        //生成预制体
        GameObject _newHitFX = Instantiate(hitFX00, _target.position + new Vector3(_xPosition, _yPosition), Quaternion.identity);
        //_newHitFX.transform.Rotate(new Vector3(0, 0, _zRotation));

        //1s后销毁
        Destroy(_newHitFX, 1f);
    }
    public void CreatHitFX01(Transform _target)
    //传入敌人位置，受击效果产生在敌人身上
    {
        //随机的位移与旋转，使得效果看起来不一样
        float _xPosition = UnityEngine.Random.Range(-0.5f, 0.5f);
        float _yPosition = UnityEngine.Random.Range(-0.5f, 0.5f);
        //float _zRotation = UnityEngine.Random.Range(-90, 90);

        //生成预制体
        GameObject _newHitFX = Instantiate(hitFX01, _target.position + new Vector3(_xPosition, _yPosition), Quaternion.identity);
        //_newHitFX.transform.Rotate(new Vector3(0, 0, _zRotation));

        //1s后销毁
        Destroy(_newHitFX, 0.5f);
    }
    #endregion

    #region DamagedFX
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
    #endregion

    #region AilmentsFX
    public void InvokeIgnitedFXFor(float _duration)
    //调用燃烧效果多长时间
    {
        //调用粒子效果
        ignitedFX.gameObject.SetActive(true);
        ignitedFX.Play();

        //调用颜色效果
        sr.color = ignitedColor;
        //经历_duration时间过后结束效果
        Invoke("CancelColorChange", _duration);
    }
    public void InvokeChilledFXFor(float _duration)
    {
        //调用粒子效果
        chilledFX.gameObject.SetActive(true);
        chilledFX.Play();

        //调用颜色效果
        sr.color = chilledColor;
        //经历_duration时间过后结束效果
        Invoke("CancelColorChange", _duration);
    }
    public void InvokeShockedFXFor(float _duration)
    {
        //调用粒子效果
        shockedFX.gameObject.SetActive(true);
        shockedFX.Play();

        //调用颜色效果
        sr.color = shockedColor;
        //经历_duration时间过后结束效果
        Invoke("CancelColorChange", _duration);
    }
    #endregion
}
