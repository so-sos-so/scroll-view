using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;


    public enum TweenType
    {
        TweenImage,
        TweenAnchorMin,
        TweenAnchorMax,
    }
    enum Axis
    {
        Horizontal = 0,
        Vertical = 1
    }

    public abstract class TweenBar
    {
        protected const float EPSILON = 0.005f;
        protected const float MAX_DELTA_TIME = 1.0F / 60;

        public RectTransform tweenRect;
        public Slider slider;
        public bool enableDownTween = false;
        public bool enbaleUpTween = false;
        
        public TweenType tweenType;
        private bool m_IsTweening = false;
        public bool isTweening
        {
            get { return m_IsTweening; }
            set { m_IsTweening = value; }
        }

        public TweenBar(TweenType type)
        {
            tweenType = type;
        }
        public virtual void Init()
        {
            if (slider == null)
            {
                Debug.LogError("TweenSlider need a slider component.");
            }
            if (tweenRect == null)
            {
                Debug.LogError("TweenSlider need a tween recttransform.");
            }
        }

        public virtual void Reset()
        {
            isTweening = false;
        }
        public virtual void Update(float elastic)
        {
            if (!isTweening)
            {
                return;
            }

            var deltaTime = Time.deltaTime;
            if (deltaTime > MAX_DELTA_TIME)
            {
                deltaTime = MAX_DELTA_TIME;
            }
            
            var diff = GetDiff();
            if (Math.Abs(diff) > EPSILON)
            {
                Step(diff * elastic * deltaTime);
            }
            else
            {
                Reset();
            }
        }
        
        public virtual void DoTween()
        {
            isTweening = true;
        }

        protected virtual float GetDiff()
        {
            return 0;
        }
        
        protected virtual void Step(float deltaValue)
        {
            
        }
    }

    public class ImageTweenBar : TweenBar
    {
        private Image m_TweenImage;
        private float m_TweenTargetFillAmount;
        public ImageTweenBar() : base(TweenType.TweenImage)
        {
        }
        public override void Init()
        {
            base.Init();
            m_TweenImage = tweenRect.GetComponent<Image>();
            m_TweenImage.type = Image.Type.Filled;
        }

        public override void Reset()
        {
            base.Reset();
            SetFillAmount(slider.normalizedValue);
        }

        public override void DoTween()
        {
            base.DoTween();
            m_TweenTargetFillAmount = slider.normalizedValue;
            var diff = GetDiff();
            if ((enableDownTween && diff < 0) || (enbaleUpTween && diff > 0))
            {
                return;
            }
            
            Reset();            
        }
        
        protected override float GetDiff()
        {
            return m_TweenTargetFillAmount - m_TweenImage.fillAmount;
        }
        
        protected override void Step(float deltaValue)
        {
            SetFillAmount(m_TweenImage.fillAmount + deltaValue);
        }
        
        void SetFillAmount(float value)
        {
            m_TweenImage.fillAmount = value;
        }
    }

    public class AnchorTweenBar : TweenBar
    {
        private Vector2 m_TweenTargetAnchor;
        private Axis m_Axis
        {
            get
            {
                var direction = slider.direction;
                return (direction == Slider.Direction.LeftToRight || direction ==Slider.Direction.RightToLeft)
                    ? Axis.Horizontal
                    : Axis.Vertical;
            }
        }
        public AnchorTweenBar(TweenType type) : base(type)
        {
        }

        public override void Reset()
        {
            base.Reset();
            var fillRect = slider.fillRect;
            tweenRect.anchorMin = fillRect.anchorMin;
            tweenRect.anchorMax = fillRect.anchorMax;
        }

        public override void DoTween()
        {
            base.DoTween();
            m_TweenTargetAnchor = GetAnchor(slider.fillRect);
            var diff = GetDiff();
            if ((enableDownTween && diff < 0) || (enbaleUpTween && diff > 0))
            {
                return;
            }
            
            Reset();
        }
        
        protected override float GetDiff()
        {
            var anchor = GetAnchor();
            return m_TweenTargetAnchor[(int) m_Axis] - anchor[(int) m_Axis];
        }
        
        protected override void Step(float deltaValue)
        {
            var anchor = Vector2.zero;
            anchor[(int) m_Axis] += deltaValue;
            SetAnchor(GetAnchor() + anchor);
        }

        Vector2 GetAnchor(RectTransform rectTrans = null)
        {
            rectTrans = rectTrans ? rectTrans : tweenRect;
            return tweenType == TweenType.TweenAnchorMin ? rectTrans.anchorMin : rectTrans.anchorMax;
        }

        void SetAnchor(Vector2 anchor, RectTransform rectTrans = null)
        {
            rectTrans = rectTrans ? rectTrans : tweenRect;
            if (tweenType == TweenType.TweenAnchorMin)
            {
                rectTrans.anchorMin = anchor;
            }
            else
            {
                rectTrans.anchorMax = anchor;
            }
        }
    }

[RequireComponent((typeof(Slider)))]
public class TweenSlider : UIBehaviour
{

    private Slider slider;
    public RectTransform tweenRect;
    [FormerlySerializedAs("TweenElastic")] public float tweenElastic = 3.0f;
    public bool tweenDown = true;
    public bool tweenUp = false;


    public float value
    {
        get { return slider.value; }
        set => Set(value, true);
    }

    public void Set(float input, bool isDoTween)
    {
        if (slider.value == input)
        {
            return;
        }

        slider.value = input;
        if (!isDoTween)
        {
            tweenBar.Reset();
            return;
        }

        tweenBar.DoTween();
    }

    public float minValue
    {
        get { return slider.minValue; }
        set
        {
            if (slider.minValue == value)
            {
                return;
            }

            slider.minValue = value;
            tweenBar.Reset();
        }
    }

    public float maxValue
    {
        get { return slider.maxValue; }
        set
        {
            if (slider.maxValue == value)
            {
                return;
            }

            slider.maxValue = value;
            tweenBar.Reset();
        }
    }

    public void SetValue(float input)
    {
        value = input;
    }

    public void SetMinValue(float input)
    {
        minValue = input;
    }

    public void SetMaxValue(float input)
    {
        maxValue = input;
    }

    bool m_ReverseValue
    {
        get
        {
            var direction = slider.direction;
            return direction == Slider.Direction.RightToLeft || direction == Slider.Direction.TopToBottom;
        }
    }

    private TweenBar tweenBar;
    private bool m_IsInit = false;

    public Slider.SliderEvent onValueChanged(Slider.SliderEvent sliderEvent)
    {
        if (!m_IsInit)
        {
            return null;
        }

        slider.onValueChanged = sliderEvent;
        return slider.onValueChanged;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateCachedReferences();
        m_IsInit = true;
    }

    protected override void Start()
    {
        if (!m_IsInit)
        {
            return;
        }

        tweenBar.Init();
        tweenBar.Reset();
    }

    private void UpdateCachedReferences()
    {
        if (tweenRect == null)
        {
            Debug.LogError("TweenSlider need a tween recttransform.");
            return;
        }

        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (tweenBar == null)
        {
            tweenBar = CreateTweenBar();
        }
    }

    private void Update()
    {
        if (!m_IsInit)
        {
            return;
        }

        tweenBar.Update(tweenElastic);
    }

    bool IsFillAmount()
    {
        var fillRect = slider.fillRect;
        var fillImage = fillRect.GetComponent<Image>();
        return fillImage && fillImage.type == Image.Type.Filled;
    }

    TweenBar CreateTweenBar()
    {
        TweenBar _tweenBar;
        if (IsFillAmount())
        {
            _tweenBar = new ImageTweenBar();
        }
        else
        {
            var type = m_ReverseValue ? TweenType.TweenAnchorMin : TweenType.TweenAnchorMax;
            _tweenBar = new AnchorTweenBar(type);
        }

        _tweenBar.slider = slider;
        _tweenBar.tweenRect = tweenRect;
        _tweenBar.enableDownTween = tweenDown;
        _tweenBar.enbaleUpTween = tweenUp;
        return _tweenBar;
    }
}
