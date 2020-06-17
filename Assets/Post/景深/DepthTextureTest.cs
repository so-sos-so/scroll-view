using UnityEngine;
using System.Collections;
 
[ExecuteInEditMode]
public class DepthTextureTest : PostEffectBase
{
    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }
 
    void OnDisable()
    {
        GetComponent<Camera>().depthTextureMode &= ~DepthTextureMode.Depth;
    }
 
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            Graphics.Blit(source, destination, _Material);
        }
    }
}