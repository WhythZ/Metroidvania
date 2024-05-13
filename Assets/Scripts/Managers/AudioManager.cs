using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //静态的全局可调用的实例
    public static AudioManager instance;

    #region SFX
    //储存音效（sound effect）的列表
    [SerializeField] AudioSource[] sfx;
    //音效播放的检测半径，太远的音效不予播放
    [SerializeField] float sfxMinPlayRadius;
    //是否可以播放音效
    private bool canPlaySFX;
    #endregion

    #region BGM
    //储存背景音乐的列表
    [SerializeField] AudioSource[] bgm;
    //是否应当（在一定时间内一直）播放某背景音乐
    public bool isPlayBGM;
    //播放的bgm的编号
    public int bgmIndex;
    #endregion

    #region CD
    //储存唱片
    [SerializeField] AudioSource[] cds;
    //当前是否在播放cd
    public bool isPlayCD;
    #endregion

    private void Awake()
    {
        //确保管理器仅有一个
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        //在进入场景0.1秒后才允许播放音效，防止开始时的SwitchToUI(ingameUI)的音效在开始就被触发
        Invoke("AllowPlaySFX", 0.1f);
    }

    private void Update()
    {
        //当未在播放cd的时候才能播放bgm
        if (!isPlayCD)
        {
            //管理bgm的播放
            if (!isPlayBGM)
                StopAllBGM();
            else
            {
                //若应当播放的bgm没有播放，开始播放（没有此if会导致一直从头开始播放哦~）
                if (!bgm[bgmIndex].isPlaying)
                    PlayBGM(bgmIndex);
            }
        }
    }

    #region SFX
    public void PlaySFX(int _sfxIndex, Transform _sfxSource)
    //传入音效编号以及音效的来源位置，若第二参数填null，则距离对音效播放的限制作废，适用于玩家自己的音效
    {
        //进入场景时，0.1秒后才允许播放音效
        if (!canPlaySFX)
            return;

        //若存在妄图播放的音效但太过遥远，则不播放
        if (_sfxSource != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _sfxSource.position) >= sfxMinPlayRadius)
            return;

        //若编号存在于列表内（编号从0开始哦）
        if(_sfxIndex < sfx.Length && sfx[_sfxIndex] != null)
        {
            //一个小trick,随机化播放目标音效的音高
            //sfx[_sfxIndex].pitch = UnityEngine.Random.Range(0.85f, 1.1f);

            //播放音效
            sfx[_sfxIndex].Play();
        }
    }
    //停止音效
    public void StopSFX(int _sfxIndex) => sfx[_sfxIndex].Stop();
    //允许播放音效
    public void AllowPlaySFX() => canPlaySFX = true;
    #endregion

    #region BGM
    public void PlayBGM(int _index)
    //传入音效编号
    {
        //若编号存在于列表内（编号从0开始哦）
        if (_index < bgm.Length)
        {
            //bgmIndex才是真正确定当前播放bgm的变量
            bgmIndex = _index;
            //播放前应当先停止播放其他所有背景音乐
            StopAllBGM();
            //播放背景音乐
            bgm[bgmIndex].Play();
        }
    }
    public void PlayRandomBGM()
    //随机播放bgm
    {
        bgmIndex = UnityEngine.Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }
    //关闭所有背景音乐
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
    #endregion

    #region CD
    public void PlayCD(int _cdIndex)
    //开始播放cd
    {
        if(_cdIndex < cds.Length)
        {
            //不允许播放bgm
            isPlayBGM = false;
            //关闭bgm
            StopAllBGM();
            //关闭其它cd
            StopAllCD();

            //播放cd
            cds[_cdIndex].Play();
        }
    }
    public void PlayRandomCD()
    //播放任意cd
    {
        //不允许播放bgm
        isPlayBGM = false;
        //关闭bgm
        StopAllBGM();
        //关闭其它cd
        StopAllCD();

        //随机抽取cd编号并播放
        int _cdIndex = UnityEngine.Random.Range(0, cds.Length);
        cds[_cdIndex].Play();
    }
    public void StopAllCD()
    //关闭所有cd，并继续之前的bgm
    {
        for (int i = 0; i < cds.Length; i++)
        {
            cds[i].Stop();
        }

        //继续播放bgm
        isPlayBGM = true;
    }
    #endregion
}