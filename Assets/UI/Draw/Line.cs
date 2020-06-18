using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Line : MaskableGraphic
{
    public Color Color
    {
        get => color;
        set => SetColor(value);
    }

    /// <summary>
    /// 线段起点。
    /// </summary>
    public Vector2 StartPosition
    {
        get => _startPosition;
        set => SetStartPosition(value);
    }

    /// <summary>
    /// 线段终点。
    /// </summary>
    public Vector2 EndPosition
    {
        get => _endPosition;
        set => SetEndPosition(value);
    }

    /// <summary>
    /// 线段宽度（像素）。
    /// </summary>
    public float Width
    {
        get => _width;
        set => SetWidth(value);
    }

    [SerializeField] Vector2 _startPosition = Vector2.zero;
    [SerializeField] Vector2 _endPosition = Vector2.one * 100;
    [SerializeField] float _width = 2;

    // 缓存
    private readonly List<UIVertex> _lineUIVertexes = new List<UIVertex>()
    {
        new UIVertex(), new UIVertex(), new UIVertex(), new UIVertex()
    };

    private readonly List<int> _lineUIVertexIndices = new List<int>()
    {
        0, 1, 2, 0, 2, 3
    };

    private GameObject go;

    public GameObject Go
    {
        get
        {
            if (null == go) go = gameObject;
            return go;
        }
    }


#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        Color = Color.magenta;
        raycastTarget = false;
    }
#endif

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        AddLineUIMesh(vh);
    }

    // 添加矩形顶点
    private void AddLineUIMesh(VertexHelper vh)
    {
        var direction = (EndPosition - StartPosition).normalized;
        var normal = Vector3.Cross(transform.forward, direction).normalized;
        var offsetV = new Vector2(normal.x, normal.y) * Width / 2;
        //var offsetH = direction * Width / 2; // 如果不考虑横向偏移，当线比较宽时转折点就会出现缺口
        var leftBottom = StartPosition - offsetV; // - offsetH;
        var leftTop = StartPosition + offsetV; // - offsetH;
        var rightTop = EndPosition + offsetV; // + offsetH;
        var rightBottom = EndPosition - offsetV; // + offsetH;
        _lineUIVertexes[0] = new UIVertex() {position = leftBottom, color = Color};
        _lineUIVertexes[1] = new UIVertex() {position = leftTop, color = Color};
        _lineUIVertexes[2] = new UIVertex() {position = rightTop, color = Color};
        _lineUIVertexes[3] = new UIVertex() {position = rightBottom, color = Color};

        // vh.AddUIVertexQuad() 方法不能利用 Graphic.color 属性
        vh.AddUIVertexStream(_lineUIVertexes, _lineUIVertexIndices);
    }

    #region 设置参数

    private void SetColor(Color color)
    {
        this.color = color;
        SetVerticesDirty();
    }

    private void SetStartPosition(Vector2 position)
    {
        _startPosition = position;
        SetVerticesDirty();
    }

    private void SetEndPosition(Vector2 position)
    {
        _endPosition = position;
        SetVerticesDirty();
    }

    private void SetWidth(float width)
    {
        _width = width;
        SetVerticesDirty();
    }

    #endregion
}
