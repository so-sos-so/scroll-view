using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(Collider2D))]
[ExecuteInEditMode]
public class Collider2DRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        var worldPoint = Vector3.zero;
        var isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _rectTransform,
            screenPos,
            eventCamera,
            out worldPoint
        );

        if (isInside)
        {
            isInside = _collider.OverlapPoint(worldPoint);
        }

        return isInside;
    }

    private Collider2D _collider;
    private RectTransform _rectTransform;
}
