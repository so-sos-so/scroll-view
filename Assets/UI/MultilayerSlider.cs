using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent((typeof(Slider)))]
public class MultilayerSlider : UIBehaviour
{
    public const int LAYERS_LIMIT = 20;
    private Slider slider;

    private float[] m_LayerDivide = new float[LAYERS_LIMIT];
    [SerializeField] private Image m_Background;

    public Image background
    {
        get { return m_Background; }
    }

    public Image fillImage
    {
        get { return slider.fillRect.GetComponent<Image>(); }
    }

    [SerializeField] [Range(1, LAYERS_LIMIT)]
    private int m_Layers = 1;

    public int layers
    {
        get => GetLayers();
        set => SetLayers(value);
    }

    private int m_CurrentLayer = 1;

    public int currentLayer
    {
        get { return m_CurrentLayer; }
        set
        {
            if (m_CurrentLayer != value)
            {
                var lastLayer = m_CurrentLayer;
                m_CurrentLayer = value;
                onLayerChanged?.Invoke(currentLayer, lastLayer);
            }
        }
    }


    [SerializeField] private float m_Value = 0;

    public float value
    {
        get { return m_Value; }
        set => SetValue(value);
    }

    [SerializeField] private float m_MaxValue = 0;

    public float maxValue
    {
        get => GetMaxValue();
        set => SetMaxValue(value);
    }

    [SerializeField] private float m_MinValue = 0;

    public float minValue
    {
        get => GetMinValue();
        set => SetMinValue(value);
    }

    public int GetLayers()
    {
        return m_Layers;
    }

    public void SetLayers(int input, bool delaySyncValue = false)
    {
        m_Layers = input;
        SeparatedLayers();
        UpdateLayers(delaySyncValue);
    }

    public void SetValue(float input, bool delaySyncValue = false)
    {
        m_Value = input;
        UpdateLayers(delaySyncValue);
    }

    public float GetMaxValue()
    {
        return layers > 1 ? m_MaxValue : slider.maxValue;
    }

    public void SetMaxValue(float input, bool delaySyncValue = false)
    {
        m_MaxValue = input;

        SeparatedLayers();
        UpdateLayers(delaySyncValue);
    }

    public float GetMinValue()
    {
        return layers > 1 ? m_MinValue : slider.minValue;
    }

    public void SetMinValue(float input, bool delaySyncValue = false)
    {
        m_MinValue = input;
        SeparatedLayers();
        UpdateLayers(delaySyncValue);
    }


    public class LayerEvent : UnityEvent<int, int>
    {
    }

    public class SliderEvent : UnityEvent<float, float, float>
    {

    }

    [SerializeField] private LayerEvent m_OnLayerChanged = new LayerEvent();

    public LayerEvent onLayerChanged
    {
        get { return m_OnLayerChanged; }
        set { m_OnLayerChanged = value; }
    }

    [SerializeField] private SliderEvent m_OnSliderValueChanged = new SliderEvent();

    public SliderEvent OnSliderValueChanged
    {
        get { return m_OnSliderValueChanged; }
        set { m_OnSliderValueChanged = value; }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        slider = GetComponent<Slider>();
        m_LayerDivide[0] = 0;
        SeparatedLayers();
    }

    protected override void OnDisable()
    {
        onLayerChanged?.RemoveAllListeners();
        OnSliderValueChanged?.RemoveAllListeners();
    }

    void SeparatedLayers()
    {
        var totalValue = maxValue - minValue;
        var singleLayerValue = (int) maxValue / layers;
        float layerValue = 0;
        for (int i = 1; i <= layers; i++)
        {
            m_LayerDivide[i] = layerValue + singleLayerValue;
            layerValue = m_LayerDivide[i];
        }

        // 余数放在最后一层
        m_LayerDivide[layers] += maxValue - layerValue;
    }

    public void UpdateLayers(bool delaySyncValue = false)
    {
        var layer = layers;
        for (int i = 1; i <= layers; i++)
        {
            if (value <= m_LayerDivide[i])
            {
                layer = i;
                break;
            }
        }

        var _minValue = m_LayerDivide[layer - 1];
        var _maxValue = m_LayerDivide[layer];
        if (_minValue != minValue || _maxValue != maxValue)
        {
            if (!delaySyncValue)
            {
                slider.minValue = m_LayerDivide[layer - 1];
                slider.maxValue = m_LayerDivide[layer];
                slider.value = value;
            }

            OnSliderValueChanged?.Invoke(_minValue, _maxValue, value);
        }

        currentLayer = layer;
    }
}
