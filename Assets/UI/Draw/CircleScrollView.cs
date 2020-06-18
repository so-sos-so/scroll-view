using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleScrollView : MonoBehaviour
{
     /// <summary>
     /// 用这个初始化
     /// </summary>
   	 void Start()
     {
         RefreshAll();
     }
    
     /// <summary>
     /// 是否是自动刷新模式，否则的话需要手动调用刷新；
     /// </summary>
     public bool IsAutoRefresh = true;

     public bool LookAtParent = false;
    
     /// <summary>
     /// 是否发生过改变；
     /// </summary>
     private bool IsChanged = false;
     /// <summary>
     /// 上一次检查的数量；
     /// </summary>
     int LastCheckCount = 0;
    
     /// <summary>
     /// Update每帧调用一次
     /// </summary>
     void Update()
     {
         //检查是否需要自动刷新；
         if (!IsAutoRefresh || Application.isPlaying)
             return;
    
         if (!IsChanged)
         {
             //检测子物体有没有被改变；
             GetChidList();
             int length = ListRect.Count;
             if (length != LastCheckCount)
             {
                 LastCheckCount = length;
                 IsChanged = true;
             }
             //此时刷新大小和位置；
             if (IsChanged)
                 ResetSizeAndPos();
         }
         else
             RefreshAll();
     }
     
    
     private void OnValidate()
     {
         //编辑器下每一次更改需要实时刷新；
         RefreshAll();
     }
    
     /// <summary>
     /// 全部刷新；
     /// </summary>
     public void RefreshAll()
     {
         GetChidList();
         ResetSizeAndPos();
     }
    
     /// <summary>
     /// 当下激活的Rect；
     /// </summary>
     private List<RectTransform> ListRect = new List<RectTransform>(4);
     List<RectTransform> tempListRect = new List<RectTransform>(4);
    
     /// <summary>
     /// 获取父节点为本身的子对象
     /// </summary>
     void GetChidList()
     {
         ListRect.Clear();
         GetComponentsInChildren(false, tempListRect);
         int length = tempListRect.Count;
         for (int i = 0; i < length; i++)
         {
             var r = tempListRect[i];
             if (r.transform.parent != transform) continue;
             ListRect.Add(r);
         }
     }
    
     /// <summary>
     /// 网格大小；
     /// </summary>
     public Vector2 CellSize = new Vector2();
    
     /// <summary>
     /// 半径；
     /// </summary>
     public float Radius = 1;
    
     /// <summary>
     /// 起始角度；
     /// </summary>
     [Range(0, 360)]
     [SerializeField]
     float m_StartAngle = 30;
    
     /// <summary>
     /// 起始角度；
     /// </summary>
     public float StartAngle
     {
         get { return m_StartAngle; }
         set
         {
             m_StartAngle = value;
             IsChanged = true;
         }
     }
    
     /// <summary>
     /// 间隔角度；
     /// </summary>
     [Range(0, 360)]
     [SerializeField]
     float m_Angle = 30;
    
     /// <summary>
     /// 间隔角度；
     /// </summary>
     public float Angle
     {
         get { return m_Angle; }
         set
         {
             m_Angle = value;
             IsChanged = true;
         }
     }
    
     /// <summary>
     /// 重新将字节点设置大小；
     /// </summary>
     public void ResetSizeAndPos()
     {
         int length = ListRect.Count;
         for (int i = 0; i < length; i++)
         {
             var tran = ListRect[i];
             tran.sizeDelta = CellSize;
             tran.anchoredPosition = GerCurPosByIndex(i);
             if (LookAtParent)
             {
                 tran.up = transform.position - tran.position;
             }
         }
     }
    
     /// <summary>
     /// 返回第几个子对象应该所在的相对位置；
     /// </summary>
     public Vector2 GerCurPosByIndex(int index)
     {
         //1、先计算间隔角度：(弧度制)
         float totalAngle = Mathf.Deg2Rad * (index * Angle + m_StartAngle);
         //2、计算位置
         Vector2 Pos = new Vector2(Radius * Mathf.Cos(totalAngle), Mathf.Sin(totalAngle) * Radius);
         return Pos;
     }
}
