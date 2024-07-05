using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    //创建相机对象
    private GameObject cam;

    //相机移动的效果
    [SerializeField] private float parallaxEffect;

    private float xPosition;

    void Start()
    {
        //赋值相机对象
        cam = GameObject.Find("Main Camera");

        //初始化为该脚本对应目标的x坐标
        xPosition = transform.position.x;
    }

    void Update()
    {
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);
    }
}
