using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapImage : Image
{
    
    
    public List<Icon> Icons = new List<Icon>();

    private UIVertex GetVertex(float x, float y, Vector2? iconTag = null, Color? color = null)
    {
        UIVertex uiVertex = new UIVertex();
        uiVertex.color = color ?? this.color;
        uiVertex.position.x = rectTransform.rect.width * (x - 0.5f);
        uiVertex.position.y = rectTransform.rect.height * (y - 0.5f);
        uiVertex.uv0 = new Vector2(x, y);
        uiVertex.uv1 = iconTag != null ? iconTag.Value / 2 + 0.5f * Vector2.one : Vector2.zero;
        return uiVertex;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
        toFill.AddUIVertexQuad(new[] {GetVertex(0, 0), GetVertex(0, 1), GetVertex(1, 1), GetVertex(1, 0)});
        lock (Icons)
        {
            for (int i = 0; i < Icons.Count; i++)
            {
                AddIconVertexArr(toFill, Icons[i]);
            }
        }
    }

    private void AddIconVertexArr(VertexHelper vh, Icon icon)
    {
        UIVertex[] vertices = new UIVertex[4];
        Vector2[] poss =
        {
            icon.LBPos,
            icon.LTPos,
            icon.RTPos,
            icon.RBPos,
        };
        vertices[0] = GetVertex(poss[0].x,poss[0].y, icon.UVLeftBottom);
        vertices[1] = GetVertex(poss[1].x,poss[1].y,
            new Vector2(icon.UVLeftBottom.x, icon.UVLeftBottom.y + icon.UVWidthLength.y));
        vertices[2] = GetVertex(poss[2].x,poss[2].y, new Vector2(icon.UVLeftBottom.x + icon.UVWidthLength.x, icon.UVLeftBottom.y + icon.UVWidthLength.y));
        vertices[3] = GetVertex(poss[3].x,poss[3].y,
            new Vector2(icon.UVLeftBottom.x + icon.UVWidthLength.x, icon.UVLeftBottom.y));
        vh.AddUIVertexQuad(vertices);
    }

    public class Icon
    {
        public Vector2 PosCenter;
        public Vector2 LBPos;
        public Vector2 LTPos;
        public Vector2 RTPos;
        public Vector2 RBPos;
        public float Angle;
        public Vector2 UVLeftBottom;
        public Vector2 UVWidthLength;

        public Icon(Vector2 posCenter, Vector2 lbPos, Vector2 ltPos, Vector2 rtPos, Vector2 rbPos, float angle, Vector2 uvLeftBottom, Vector2 uvWidthLength)
        {
            PosCenter = posCenter;
            LBPos = lbPos;
            LTPos = ltPos;
            RTPos = rtPos;
            RBPos = rbPos;
            Angle = angle;
            UVLeftBottom = uvLeftBottom;
            UVWidthLength = uvWidthLength;
        }
    }
}


