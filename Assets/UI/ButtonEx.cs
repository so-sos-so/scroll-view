using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class ButtonEx : Button
{
    [Serializable]
    public class ButtonLongClickedEvent : UnityEvent
    {
        internal bool IsPressing()
        {
            return null != _routine;
        }

        internal bool IsLongClickedHappend()
        {
            return _happend;
        }

        internal void CleanHappendFlag()
        {
            _happend = false;
        }

        internal void StartCoroutine(ButtonEx button)
        {
            _happend = false;
            _routine = _CoLongClick();
            button.StartCoroutine(_routine);
        }

        internal void KillCoroutine(ButtonEx button)
        {
            if (null != _routine)
            {
                button.StopCoroutine(_routine);
                _routine = null;
            }
        }

        private IEnumerator _CoLongClick()
        {
            var dueTime = Time.unscaledTime + holdTime;
            while (Time.unscaledTime < dueTime)
            {
                yield return null;
            }

            if (IsPressing())
            {
                _routine = null;
                Invoke();
                _happend = true;
            }
        }

        [Tooltip("How long must pointer be down on this object to trigger a long click")]
        public float holdTime = 0.5f;

        private IEnumerator _routine;
        private bool _happend = false;
    }

    protected override void Awake()
    {
        base.Awake();

        _SetHasLongClick(hasOnLongClick);
    }

    private void _Press()
    {
        if (!IsActive() || !IsInteractable())
        {
            return;
        }

        onClick.Invoke();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (null != onLongClick)
        {
            if (onLongClick.IsLongClickedHappend() && IsMutexClick)
            {
                onLongClick.CleanHappendFlag();
                return;
            }

            onLongClick.CleanHappendFlag();
        }

        _Press();
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        _Press();

        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
        {
            return;
        }

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(_OnFinishSubmit());
    }

    private IEnumerator _OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //IsPressed会引起失去焦点后第一次点击判定为false的问题，使得第一次长按失效。
        //IsPressed会判断isActive&&isPointerEnter&&isPointerDown
        //具体原因是当游戏屏幕区域没有焦点的时候，第一次点击时的执行顺序为
        //OnPointerDown->OnPointerEnter
        //这就造成了如果我在下面判断IsPressed的话会得到false，因为这个时候OnPointerEnter还没执行
        //以上问题发生在Windows平台，其余平台暂未测试
        if (null != onLongClick /*&& IsPressed()*/)
        {
            onLongClick.StartCoroutine(this);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (null != onLongClick && onLongClick.IsPressing())
        {
            onLongClick.KillCoroutine(this);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (null != onLongClick)
        {
            if (onLongClick.IsPressing())
            {
                onLongClick.KillCoroutine(this);
            }
        }
    }

    private void _SetHasLongClick(bool val)
    {
        _hasLongClick = val;

        if (_hasLongClick)
        {
            onLongClick = onLongClick ?? new ButtonLongClickedEvent();
        }
        else
        {
            onLongClick = null;
        }
    }

    public bool hasOnLongClick
    {
        get { return _hasLongClick; }
        set
        {
            if (_hasLongClick != value)
            {
                _SetHasLongClick(value);
            }
        }
    }

    public ButtonLongClickedEvent onLongClick;
    [Tooltip("长按触发时是否忽略普通点击事件")] public bool IsMutexClick = false;

    [SerializeField] private bool _hasLongClick = true;
}
