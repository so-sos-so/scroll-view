using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


[RequireComponent(typeof(VideoPlayer))]
public class UIVideoPlayer : MonoBehaviour
{
    private VideoPlayer _videoPlayer = null;
    private RenderTexture _renderTexture = null;
    private RawImage _imgVideo = null;
    private string _videoName = String.Empty;

    private Action<string> _onEndCallback = null;
    private Action<string> _onStartCallback = null;

    public bool IsPlaying
    {
        get
        {
            if (_videoPlayer != null)
            {
                return _videoPlayer.isPlaying;
            }

            return false;
        }
    }

    void Awake()
    {
        Init();
        enabled = false;
    }

    private void OnEnable()
    {
        if (_renderTexture == null)
        {
            Debug.LogWarning("VidePlayer enbale failed, rendertexture got null");
            return;
        }

        _videoPlayer.targetTexture = _renderTexture;
        _videoPlayer.Prepare();
    }

    private void OnDestroy()
    {
        if (_renderTexture != null)
        {
            UnityEngine.Object.DestroyImmediate(_renderTexture);
            _renderTexture = null;
        }
    }


    public void PlayVideo(string videoName, bool isLoop, Action<string> onStartAction, Action<string> onEndAction)
    {
        if (_videoPlayer == null) return;

        if (_videoPlayer.isPlaying)
            StopVideo();


        if (_imgVideo != null)
            _imgVideo.texture = _renderTexture;

        string videoPath = Path.Combine(Application.streamingAssetsPath, videoName);
        _videoName = videoName;
        _videoPlayer.url = videoPath;
        _videoPlayer.isLooping = isLoop;
        _onStartCallback = onStartAction;
        _onEndCallback = onEndAction;

        enabled = true;
    }

    public void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
    {
        if (null != _imgVideo)
        {
            _imgVideo.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
        }
    }

    public void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
    {
        if (null != _imgVideo)
        {
            _imgVideo.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
        }
    }

    public void SetCanvasRendererColor(Color color)
    {
        if (null != _imgVideo)
        {
            _imgVideo.canvasRenderer.SetColor(color);
        }
    }

    public void SetCanvasRendererAlpha(float alpha)
    {
        if (null != _imgVideo)
        {
            _imgVideo.canvasRenderer.SetAlpha(alpha);
        }
    }

    private void Init()
    {
        _imgVideo = gameObject.GetComponent<RawImage>();
        _videoPlayer = gameObject.GetComponent<VideoPlayer>();
        _videoPlayer.playOnAwake = false;

        _videoPlayer.waitForFirstFrame = false;
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        _videoPlayer.aspectRatio = VideoAspectRatio.FitOutside;
        _videoPlayer.source = VideoSource.Url;

        _videoPlayer.prepareCompleted += Prepared;
        _videoPlayer.loopPointReached += EndReached;
        _videoPlayer.errorReceived += OnVideoError;
        _videoPlayer.started += OnStarted;

        _renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        _renderTexture.name = "VideoPlayerRT";
    }

    public void StopVideo()
    {
        if (_videoPlayer == null) return;

        _videoPlayer.Stop();
        OnVideoStop();
    }

    private void OnVideoStop()
    {
        if (_imgVideo != null)
            _imgVideo.texture = null;

        _onEndCallback = null;
        _videoName = string.Empty;

        enabled = false;
    }

    #region VideoPlayer回调

    private void Prepared(VideoPlayer player)
    {
        player.Play();
        if (_onStartCallback != null)
        {
            _onStartCallback.Invoke(_videoName);
            _onStartCallback = null;
        }
    }

    private void EndReached(VideoPlayer player)
    {
        if (_onEndCallback != null)
        {
            _onEndCallback.Invoke(_videoName);
            _onEndCallback = null;
        }

        if (!player.isLooping)
            OnVideoStop();
    }

    private void OnVideoError(VideoPlayer player, string message)
    {
        Debug.LogError($"VideoPlayer got error: {message}");
        StopVideo();
    }

    private void OnStarted(VideoPlayer player)
    {
    }

    #endregion
}
