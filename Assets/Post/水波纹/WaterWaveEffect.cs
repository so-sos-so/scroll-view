using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterWaveEffect : PostEffectBase
{
    [Range(0,1)]
    public float _distanceFactor;
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_Material)
        {
            _Material.SetFloat("_distanceFactor", _distanceFactor);
            Graphics.Blit(src, dest, _Material);
        }
    }
}
