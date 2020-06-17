using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class WaterWaveEffect : PostEffectBase
{
    [Range(0,1)]
    public float _distanceFactor = 0.5f;
    public float _timeFactor = -10;
    public Vector2 ScreenCenter = Vector2.one * 0.5f;
    private float _waveDistance = 0;
    [Range(0,1)]
    public float _waveWidth = 0.3f;
    private float startTime;
    public float TimeSpeed = 1;
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        _waveDistance = (Time.time - startTime) * TimeSpeed;
        if (_Material)
        {
            _Material.SetFloat("_distanceFactor", _distanceFactor);
            _Material.SetFloat("_timeFactor", _timeFactor);
            _Material.SetFloat("_waveDistance", _waveDistance);
            _Material.SetFloat("_waveWidth", _waveWidth);
            _Material.SetVector("_screenCenter", ScreenCenter);
            Graphics.Blit(src, dest, _Material);
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            ScreenCenter = new Vector2(mousePos.x / Screen.width,mousePos.y / Screen.height);
            startTime = Time.time;
        }   
    }
}
