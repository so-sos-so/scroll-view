using System.Collections;
using UnityEngine;

namespace Framework
{
    public static class ScreenSnap
    {
        //全屏截图
        public static void ScreenShot(string fileName)
        {
            ScreenCapture.CaptureScreenshot(fileName);
        }
        
        public static IEnumerator CaptureByRect(Rect mRect,string mFileName)  
        {  
            //等待渲染线程结束  
            yield return new WaitForEndOfFrame();  
            //初始化Texture2D  
            Texture2D mTexture=new Texture2D((int)mRect.width,(int)mRect.height,TextureFormat.RGB24,false);  
            //读取屏幕像素信息并存储为纹理数据  
            mTexture.ReadPixels(mRect,0,0);  
            //应用  
            mTexture.Apply();  
          
            //将图片信息编码为字节信息  
            byte[] bytes = mTexture.EncodeToPNG();    
            //保存  
            System.IO.File.WriteAllBytes(mFileName, bytes);  
          
            //如果需要可以返回截图  
            //return mTexture;  
        } 
        
        public static IEnumerator CaptureByCamera(Camera mCamera,Rect mRect,string mFileName)  
        {  
            //等待渲染线程结束  
            yield return new WaitForEndOfFrame();  
  
            //初始化RenderTexture  
            RenderTexture mRender=new RenderTexture((int)mRect.width,(int)mRect.height,24); 
            //设置相机的渲染目标  
            mCamera.targetTexture=mRender;  
            //开始渲染  
            mCamera.Render();  
          
            //激活渲染贴图读取信息  
            RenderTexture.active=mRender;  
          
            Texture2D mTexture=new Texture2D((int)mRect.width,(int)mRect.height,TextureFormat.RGB24,false);  
            //读取屏幕像素信息并存储为纹理数据  
            mTexture.ReadPixels(mRect,0,0);  
            //应用  
            mTexture.Apply();  
          
            //释放相机，销毁渲染贴图  
            mCamera.targetTexture = null;     
            RenderTexture.active = null;   
            GameObject.Destroy(mRender);    
          
            //将图片信息编码为字节信息  
            byte[] bytes = mTexture.EncodeToPNG();    
            //保存  
            System.IO.File.WriteAllBytes(mFileName,bytes);  
          
            //如果需要可以返回截图  
            //return mTexture;  
        }
    }
}