using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var _sprites = Resources.LoadAll<Sprite>(GetAtlasPath("icons/icon0"));
        var mainSprite = Resources.Load<Sprite>("icons");
    }
    
    private string GetAtlasPath(string path)
    {
        int index = path.LastIndexOf("/");
        path = path.Substring(0, index);
        return path;
    }
}
