using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomScript : MonoBehaviour
{
    public Material material = null;

    [Range(-2.0f,2.0f),Tooltip("放大强度")]
    public float fZoomFfactor = 0.4f;

    [Range(0.0f, 0.2f), Tooltip("放大镜的半径")]
    public float fCircleRadius = 0.15f;

    [Range(0.0001f, 0.1f), Tooltip("放大镜的边缘强度")]
    public float fCircleEdgeStrength = 0.05f;

    private Vector2 vec2MousePos = Vector2.zero;
    

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;
            //转换屏幕坐标系区间在[0,1]，用于符合shader uv的区间。
            vec2MousePos.x = mousePos.x / Screen.width;
            vec2MousePos.y = mousePos.y/Screen.height;

            Debug.Log("x:["+ vec2MousePos.x + "] y:[" + vec2MousePos.y + "]");
        }
    }

     void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material)
        {
            material.SetVector("_MousePos", vec2MousePos);
            material.SetFloat("_ZoomFactor", fZoomFfactor);
            material.SetFloat("_CircleRadius", fCircleRadius);
            material.SetFloat("_CircleEdgeStrength", fCircleEdgeStrength);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
