using UnityEngine;
using System.Collections;
 
//编辑状态下也运行
[ExecuteInEditMode]
//继承自PostEffectBase
public class SimpleBlurEffect : PostEffectBase
{
    //模糊半径
    public float BlurRadius = 1.0f;
    //降分辨率
    public int downSample = 2;
    //迭代次数
    public int iteration = 3;
 
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        if (_Material)
        {
            //申请RenderTexture，RT的分辨率按照downSample降低
            RenderTexture rt1 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);
            RenderTexture rt2 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);
            
            //直接将原图拷贝到降分辨率的RT上
            Graphics.Blit(source, rt1);
 
            //进行迭代，一次迭代进行了两次模糊操作，使用两张RT交叉处理
            for(int i = 0; i < iteration; i++)
            {
                //用降过分辨率的RT进行模糊处理
                _Material.SetFloat("_BlurRadius", BlurRadius);
                Graphics.Blit(rt1, rt2, _Material);
                Graphics.Blit(rt2, rt1, _Material);
            }
 
            //将结果拷贝到目标RT
            Graphics.Blit(rt1, destination);
            RenderTexture renderTexture = new RenderTexture(1,1,1,RenderTextureFormat.DefaultHDR);
            //释放申请的两块RenderBuffer内容
            RenderTexture.ReleaseTemporary(rt1);
            RenderTexture.ReleaseTemporary(rt2);
        }
        
    }
}