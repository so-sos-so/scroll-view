using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI点击事件 包含onClick onDown onEnter onExit onUp onSelect onUpdateSelect
/// </summary>
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate();
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onDrag;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp();
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect();
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect();
    }
}

