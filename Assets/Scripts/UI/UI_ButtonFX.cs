using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UI_ButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlaySFX(9, null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
