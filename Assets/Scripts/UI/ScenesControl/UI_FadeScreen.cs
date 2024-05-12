using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FadeScreen : MonoBehaviour
{
    [SerializeField] private Animator anim;

    //动画fadeOut和fadeIn在Animator内对应的parameter是Trigger类型的
    public void FadeOut() => anim.SetTrigger("fadeOut");

    public void FadeIn() => anim.SetTrigger("fadeIn");
}
