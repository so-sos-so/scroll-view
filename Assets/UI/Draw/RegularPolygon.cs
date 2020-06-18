using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 正多边形
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class RegularPolygon : MaskableGraphic
{
    /// <summary>
    /// 正多边形填充颜色。
    /// </summary>
    public Color Color
    {
        get => color;
        set => SetColor(value);
    }

    /// <summary>
    /// 正多边形中心点。
    /// </summary>
    public Vector2 Center
    {
        get => _center;
        set => SetCenter(value);
    }

    /// <summary>
    /// 正多边形半径（中心点到顶点的距离）。
    /// </summary>
    public float Radius
    {
        get => _radius;
        set => SetRadius(value);
    }

    /// <summary>
    /// 旋转角度。
    /// </summary>
    public float Rotation
    {
        get => _rotation;
        set => SetRotation(value);
    }

    /// <summary>
    /// 正多边形边数（不小于3）。
    /// </summary>
    public byte SideCount
    {
        get => _sideCount;
        set => SetSideCount(value);
    }

    [SerializeField] Vector2 _center = Vector2.one * 100;
    [SerializeField] float _radius = 100;
    [SerializeField] float _rotation = 0;
    [SerializeField] [Range(3, 64)] byte _sideCount = 8;

    // 缓存
    private readonly List<UIVertex> _circleUIVertexes = new List<UIVertex>();
    private readonly List<int> _circleUIVertexIndices = new List<int>();


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
        AddRegularPolygonUIMesh(vh);
    }

    // 添加正多边形
    private void AddRegularPolygonUIMesh(VertexHelper vh)
    {
        _circleUIVertexes.Clear();
        _circleUIVertexIndices.Clear();

        var vertexAngle = 2 * Mathf.PI / SideCount;
        var centerUIVertex = new UIVertex() {position = Center, color = Color};
        _circleUIVertexes.Add(centerUIVertex);
        var index = 0;
        while (index < SideCount)
        {
            var totalAngle = vertexAngle * index;
            var offsetH = Radius * Mathf.Sin(totalAngle);
            var offsetV = Radius * Mathf.Cos(totalAngle);
            var vertexPosition = Center + new Vector2(offsetH, offsetV);
            if (Rotation != 0)
            {
                vertexPosition = RotateVertex(vertexPosition, Rotation);
            }

            var uiVertex = new UIVertex() {position = vertexPosition, color = Color};
            _circleUIVertexes.Add(uiVertex);
            _circleUIVertexIndices.Add(0);
            _circleUIVertexIndices.Add(index);
            _circleUIVertexIndices.Add(index + 1);
            index++;
        }

        // 第一个三角形的顶点也要作为左后一个三角形的顶点
        var firstSideUIVertex = _circleUIVertexes[1];
        _circleUIVertexes.Add(firstSideUIVertex);
        _circleUIVertexIndices.Add(0);
        _circleUIVertexIndices.Add(index);
        _circleUIVertexIndices.Add(index + 1);

        vh.AddUIVertexStream(_circleUIVertexes, _circleUIVertexIndices);
    }

    // 旋转顶点
    private Vector2 RotateVertex(Vector2 source, float angle)
    {
        //var rotation = Quaternion.AngleAxis(angle, -transform.forward);
        var rotation = Quaternion.Euler(0, 0, -angle);
        var direction = source - Center;
        var result = rotation * direction;
        var destination = new Vector2(result.x, result.y) + Center;

        return destination;
    }

    #region 设置参数

    private void SetColor(Color color)
    {
        this.color = color;
        SetVerticesDirty();
    }

    private void SetCenter(Vector2 center)
    {
        _center = center;
        SetVerticesDirty();
    }

    private void SetRadius(float radius)
    {
        _radius = radius;
        SetVerticesDirty();
    }

    private void SetRotation(float rotation)
    {
        _rotation = rotation;
        SetVerticesDirty();
    }

    private void SetSideCount(byte sideCount)
    {
        if (SideCount < 3)
        {
            Debug.LogError($"RegularPolygon's {nameof(SideCount)} must be equal or greather than 3.");
            return;
        }

        _sideCount = sideCount;
        SetVerticesDirty();
    }

    #endregion
}
