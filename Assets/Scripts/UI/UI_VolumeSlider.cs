using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
//注意这里多调用了一个UI相关的
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    //控制音量的滑块
    public Slider slider;
    //链接到AudioMixer参数，此参数在我们的设置下是与AudioMixer的Volume值相关联的
    //这个值在Hierarchy内手动填写，注意名字要与我们设置参数时的名字一致，如"Volume_SFX"
    public string parameter;

    //手动链接到Asset中的混音器，即Unity中控制音量大小等的Window
    [SerializeField] private AudioMixer audioMixer;
    //调整Slider的滑动对音量变化的显著性，常用25
    [SerializeField] private float multiplier;

    private void Start()
    {
        slider = GetComponent<Slider>();
        //防止滑块拖到最底端的时候，AudioMixer的Volume反弹回0，即音频音量反而变成最大
        slider.minValue = 0.001f;
    }

    public void LoadSlider(float _value)
    //加载存档时加载之前调节过的音量
    {
        //这里是参考上面的slider.minValue = 0.001f;的数值
        //这里要有等于号，不然把音量拉到最底（如得到0.0010000123f）会直接不记录该数值
        if (_value >= 0.001f)
        {
            slider.value = _value;
        }
    }

    public void LinkSliderValueToAudioMixer(float _value)
    //用于将AudioMixer的Volume值与Slider的值关联在一起
    {
        //这里用到的公式挺好用
        audioMixer.SetFloat(parameter, Mathf.Log10(_value) * multiplier);
    }
}