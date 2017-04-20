using UnityEngine;
using UnityEngine.UI;

public class LayerManager
{
    private static Transform _topUILayer;
    private static Transform _actorLayer;
    private static Transform _mapLayer;
    private static Transform _actorUILayer;
    private static Transform _uiLayer;
    private static Canvas _uiCanvas;
    public static void Setup()
    {
        GameObject obj = new GameObject();
        obj.name = "TopCanvas";

        Canvas canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1136, 640);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        //scaler.matchWidthOrHeight = 1;

        obj.AddComponent<GraphicRaycaster>();
        //_topUILayer = GameObject.Find("TopCanvas").transform;
        _topUILayer = obj.transform;
        GameObject.DontDestroyOnLoad(_topUILayer.gameObject);
        
    }

    public static Transform topUILayer
    {
        get
        {
            return _topUILayer;
        }
    }

    public static void SetUILayer(Transform value)
    {
        _uiLayer = value;
        _uiCanvas = _uiLayer.GetComponent<Canvas>();
    }

    public static void SetActorLayer(Transform value)
    {
        _actorLayer = value;
    }

    public static void SetMapLayer(Transform value)
    {
        _mapLayer = value;
    }

    public static void ClearLayers()
    {
        _uiLayer = null;
        _actorLayer = null;
        _actorUILayer = null;
        _uiCanvas = null;
        _mapLayer = null;
    }
    public static Transform mapLayer
    {
        get
        {
            return _mapLayer;
        }
    }
    public static RectTransform uiLayer
    {
        get
        {
            return _uiLayer as RectTransform;
        }
    }

    public static Canvas uiCanvas
    {
        get
        {
            return _uiCanvas;
        }
    }

    public static Transform actorLayer
    {
        get
        {
            if(_actorLayer == null)
            {
                _actorLayer = GameObject.Find("ActorLayer").transform;
            }
            return _actorLayer;
        }
    }

    public static Transform actorUILayer
    {
        get
        {
            if (_actorUILayer == null)
            {
                _actorUILayer = GameObject.Find("ActorUILayer").transform;
            }
            return _actorUILayer;
        }
        
    }
}
