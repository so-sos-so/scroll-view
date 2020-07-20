using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextMeshGenerator
{
    private Font font;
    private string text = "你好";
    private Canvas canvas;
    private FontData fontData = FontData.defaultFontData;
    private TextGenerationSettings textGenerationSettings;
    private TextGenerator textGenerator = new TextGenerator();
    /// <summary>
    /// xy : center.xy  zw : width height
    /// </summary>
    public Vector4 Rect;
    private Vector2 roundingOffset;
    private RectTransform transform;
    private List<UIVertex> results = new List<UIVertex>();
    private readonly UIVertex[] emptyVertex = new UIVertex[0];
    private Color color;
    private int scale;
    
    public TextMeshGenerator(Font font, Canvas canvas, RectTransform transform)
    {
        this.font = font;
        this.canvas = canvas;
        this.transform = transform;
    }

    public UIVertex[] Generator(string text, Vector2 pos, Color color = default, int fontSize = 14, int scale = 10)
    {
        this.color = color == default ? Color.black : color;
        this.text = text;
        this.scale = scale;
        fontData.fontSize = fontSize * scale;
        MoveTo(pos);
        return Generator();
    }
    
    private UIVertex[] Generator()
    {
        if (font == null)
            return emptyVertex;

        Vector2 extents = transform.rect.size * scale;

        var settings = GetGenerationSettings(extents);
        textGenerator.PopulateWithErrors(text, settings, transform.gameObject);

        // Apply the offset to the vertices
        IList<UIVertex> verts = textGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            return emptyVertex;
        }
            
        results.Clear();
        for (int i = 0; i < vertCount; ++i)
        {
            var vertex = verts[i];
            vertex.position *= unitsPerPixel;
            vertex.uv1 = new Vector2((vertex.uv0 / 2).x + 0.5f, (vertex.uv0 / 2).y + 0.5f);
            vertex.position.x += roundingOffset.x;
            vertex.position.y -= roundingOffset.y;
            results.Add(vertex);
        }
        return results.ToArray();
    }
    
    private void MoveTo(Vector2 pos)
    {
        Vector2 extents = transform.rect.size;
        
        var settings = GetGenerationSettings(extents);
        textGenerator.PopulateWithErrors(text, settings, transform.gameObject);
        var width = textGenerator.GetPreferredWidth(text, settings);
        var height = textGenerator.GetPreferredHeight(text, settings);
        Rect = new Vector4(transform.rect.width * pos.x, transform.rect.height * pos.y, width, height);
        roundingOffset = (Vector2) Rect - new Vector2(width / 2, height / 2);
    }
    
    private float pixelsPerUnit
    {
        get
        {
            var localCanvas = canvas;
            if (!localCanvas)
                return 1;
            if (!font || font.dynamic)
                return localCanvas.scaleFactor;
            if (fontData.fontSize <= 0 || font.fontSize <= 0)
                return 1;
            return font.fontSize / (float) fontData.fontSize;
        }
    }
    
    private TextGenerationSettings GetGenerationSettings(Vector2 extents)
    {
        var settings = new TextGenerationSettings();

        settings.generationExtents = extents;
        if (font != null && font.dynamic)
        {
            settings.fontSize = fontData.fontSize;
            settings.resizeTextMinSize = fontData.minSize;
            settings.resizeTextMaxSize = fontData.maxSize;
        }

        // Other settings
        settings.textAnchor = fontData.alignment;
        settings.alignByGeometry = fontData.alignByGeometry;
        settings.scaleFactor = pixelsPerUnit;
        settings.color = color;
        settings.font = font;
        settings.pivot = transform.pivot;
        settings.richText = fontData.richText;
        settings.lineSpacing = fontData.lineSpacing;
        settings.fontStyle = fontData.fontStyle;
        settings.resizeTextForBestFit = fontData.bestFit;
        settings.updateBounds = false;
        settings.horizontalOverflow = fontData.horizontalOverflow;
        settings.verticalOverflow = fontData.verticalOverflow;

        return settings;
    }
}
