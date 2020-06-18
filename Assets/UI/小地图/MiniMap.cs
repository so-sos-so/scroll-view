using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework;
using UnityEngine;
using UnityEngine.U2D;

public class MiniMap : MonoBehaviour
{
    public enum MapType
    {
        Radar,
        Map,
    }

    public MapType Type;
    private MinimapImage MinimapImage;
    private Vector2 imgMapLB
    {
        get
        {
            Vector2 lb = new Vector2(playerPos.x - uiMapToRealWidth / 2,
                playerPos.z - uiMapToRealHeight / 2);
            if (playerPos.x + uiMapToRealWidth / 2 > mapWidth)
            {
                lb.x = mapWidth - uiMapToRealWidth;
            }
            else if (playerPos.x - uiMapToRealWidth / 2 < 0)
            {
                lb.x = uiMapToRealWidth / 2;
            }
            
            if (playerPos.z + uiMapToRealHeight / 2 > mapHeight)
            {
                lb.y = mapHeight - uiMapToRealHeight;
            }
            else if (playerPos.z - uiMapToRealHeight / 2 < 0)
            {
                lb.y = uiMapToRealHeight / 2;
            }
            return lb;
        }
    }

    private Vector3 originMapCenter;
    private Vector3 playerPos;
    //UI map的宽高
    private float uiMapHeight => MinimapImage.rectTransform.rect.height;
    private float uiMapWidth => MinimapImage.rectTransform.rect.width;
    //对应真是地图的宽 高
    private float uiMapToRealHeight => uiMapToRealWidth * uiMapHeight / uiMapWidth;
    private float uiMapToRealWidth;

    private float mapWidth;
    private float mapHeight;

    private Vector2 uvScale;

    private SpriteAtlasAsset spriteAtlasAsset;
    private bool isSpriteLoaded;
    
    private List<MinimapIcon> Icons = new List<MinimapIcon>(); 
    
    private void Awake()
    {
        MinimapImage = GetComponent<MinimapImage>();
    }

    public void SetMapWidthHeight(float width, float height)
    {
        mapWidth = width;
        mapHeight = height;
    }

    public void SetUI2RealScale(float widthScale)
    {
        uiMapToRealWidth = widthScale;
        uvScale = new Vector2(uiMapToRealWidth / mapWidth, uiMapToRealHeight / mapHeight);
    }

    public void SetOriginMapCenter(Vector3 mapCenter)
    {
        originMapCenter = mapCenter;
    }

    public void SetPlayerPos(Vector3 pos)
    {
        playerPos = pos;
    }
    
    public void UpdateIcons(List<MinimapIcon> icons)
    {
        Icons.Clear();
        Icons.AddRange(icons);
        MinimapImage.Icons.Clear();
        foreach (var icon in icons)
        {
            AddIcon(icon.Pos, icon.Radius, icon.Dir, icon.SpritePath, false);
        }
        MinimapImage.SetVerticesDirty();
    }

    public void UpdateMapUV()
    {
        Vector2 lb = imgMapLB;
        Vector2 playerOffset = new Vector2(lb.x + uiMapToRealWidth / 2, lb.y + uiMapToRealHeight / 2);
        float widthRate = playerOffset.x / mapWidth;
        float heightRate = playerOffset.y / mapHeight;
        SetMapUV(new Vector4(uvScale.x, uvScale.y, widthRate - uvScale.x / 2, heightRate - uvScale.y / 2));
    }

    private Vector2 GetIconPos(Vector3 pos)
    {
        float widthRate = 1;
        float heightRate = 1;
        switch (Type)
        {
            case MapType.Radar:
                var offsetPos = new Vector2(pos.x, pos.z) - imgMapLB;
                widthRate = offsetPos.x / uiMapToRealWidth;
                heightRate = 1 - offsetPos.y / uiMapToRealHeight;
                break;
            case MapType.Map:
                var offset = pos - originMapCenter;
                widthRate = offset.x / mapWidth;
                heightRate = offset.z / mapHeight;
                break;
        }
        return new Vector2(widthRate, heightRate);
    }

    private void AddIcon(Vector3 pos, float radius, Vector3 dir, string spritePath, bool isUpdateVertices = true)
    {
        if (spriteAtlasAsset == null)
        {
            InitSpriteAsset(spritePath);
        }
        Icons.Add(new MinimapIcon(pos, radius, dir, spritePath));
        var (uvLb, wl) = CalcUV(spriteAtlasAsset.GetSprite(GetSpriteName(spritePath)));
        var iconPos = GetIconPos(pos);
        if(Type == MapType.Radar && (iconPos.x > 1 || iconPos.y > 1)) return;
        float angle = Vector3.Angle(Vector3.forward, dir);
        Vector3 normal = Vector3.Cross (dir, Vector3.forward);
        angle *= Mathf.Sign (Vector3.Dot(normal,Vector3.up));
        MinimapImage.Icons.Add(AdjustIconPos(iconPos, radius, angle, uvLb, wl));
        if (isUpdateVertices)
            MinimapImage.SetVerticesDirty();
    }

    /// <summary>
    /// 调整icon的旋转，还有因为地图宽高不一致造成的icon拉伸
    /// </summary>
    private MinimapImage.Icon AdjustIconPos(Vector2 center, float radius, float angle, Vector2 uvLb, Vector2 wl)
    {
        Matrix4x4 rotateMat = Matrix4x4.Translate(center) * Matrix4x4.Rotate(Quaternion.Euler(0, 0, angle)) *
                              Matrix4x4.Translate(-center);
        Vector2 lb = rotateMat * new Vector4(center.x - radius / 2, center.y - radius / 2,1,1);
        Vector2 lt = rotateMat * new Vector4(center.x - radius / 2, center.y + radius / 2,1,1);
        Vector2 rt = rotateMat * new Vector4(center.x + radius / 2, center.y + radius / 2,1,1);
        Vector2 rb = rotateMat * new Vector4(center.x + radius / 2, center.y - radius / 2,1,1);
        float rate = uiMapWidth / uiMapHeight;
        lb.y = center.y + (lb.y - center.y) * rate;
        lt.y = center.y + (lt.y - center.y) * rate;
        rt.y = center.y + (rt.y - center.y) * rate;
        rb.y = center.y + (rb.y - center.y) * rate;
        var icon = new MinimapImage.Icon(center, lb, lt, rt, rb, angle, uvLb, wl);
        return icon;
    }
    

    private void InitSpriteAsset(string path)
    {
        path = GetAtlasPath(path);
        spriteAtlasAsset = new SpriteAtlasAsset(path);
        MinimapImage.material.SetTexture("_IconTex", spriteAtlasAsset.Texture);
    }

    private string GetAtlasPath(string path)
    {
        int index = path.LastIndexOf("/");
        path = path.Substring(0, index);
        return path;
    }

    private string GetSpriteName(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    private (Vector2 uvLb, Vector2 wl) CalcUV(Sprite sprite)
    {
        Rect UVs = sprite.rect;
        UVs.x /= sprite.texture.width;
        UVs.width /= sprite.texture.width;
        UVs.y /= sprite.texture.height;
        UVs.height /= sprite.texture.height;
        return (UVs.position, UVs.size);
    }

    private void SetMapUV(Vector4 st)
    {
        MinimapImage.material.SetVector("_MainTex_ST", st);
    }

    public struct MinimapIcon
    {
        public Vector3 Pos;
        public float Radius;
        public Vector3 Dir;
        public string SpritePath;

        public MinimapIcon(Vector3 pos, float radius, Vector3 dir, string spritePath)
        {
            Pos = pos;
            Radius = radius;
            Dir = dir;
            SpritePath = spritePath;
        }
    }

}

public class SpriteAtlasAsset
{
    private Dictionary<string, Sprite> sprites;
    public Texture Texture => sprites.Count > 0 ? sprites.First().Value.texture : null;
    public SpriteAtlasAsset(string atlasPath)
    {
        var _sprites = Resources.LoadAll<Sprite>(atlasPath);
        var mainSprite = Resources.Load<Sprite>(atlasPath);
        sprites = new Dictionary<string, Sprite>(_sprites.Length);
        foreach (var sprite in _sprites)
        {
            sprites.Add(sprite.name, sprite);
        }
    }

    public Sprite GetSprite(string name)
    {
        return sprites.TryGetValue(name, out var sprite) ? sprite : null;
    }
}
