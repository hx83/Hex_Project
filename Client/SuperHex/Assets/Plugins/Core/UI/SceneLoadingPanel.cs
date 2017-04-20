using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SceneLoadingPanel : MonoBehaviour, IEventDispatcher
{
    private const float SPEED = 0.02f;
    private Text _progresstxt;
    private Text _titleTxt;
    private RectTransform _barBg;
    private RectTransform _bar;
    private Image _bg;
    private float add;
    private float targetP;
    private float p = 0;
    private bool isComplete;
    private bool _isShow;
    private float _width;
    private Vector2 _offsetHelp;
    void Awake()
    {
        _instance = this;
        _dispatcher = new EventDispatcher();
        _progresstxt = this.transform.FindChild("ProgressTxt").GetComponent<Text>();
        _titleTxt = this.transform.FindChild("TitleTxt").GetComponent<Text>();
        _barBg = this.transform.FindChild("BarBg") as RectTransform;
        _width = _barBg.sizeDelta.x;
        _bar = _barBg.FindChild("Bar") as RectTransform;
        _offsetHelp = _bar.offsetMax;
        _offsetHelp.x = -_width;
        _bar.offsetMax = _offsetHelp;
        _bg = this.transform.GetComponent<Image>();
        add = SPEED;
        _isShow = true;
    }

    public static SceneLoadingPanel Instance
    {
        get
        {
            return _instance;
        }
    }

    public void SetTitle(string title)
    {
        _titleTxt.text = title;
    }

    public void SetPercent(float targetP)
    {
        this.targetP = targetP;
        if(targetP == 1)
        {
            add = SPEED * 4;
        }
    }

    public bool IsShow
    {
        get
        {
            return _isShow;
        }
    }

    void FixedUpdate()
    {
        if (isComplete)
        {
            return;
        }

        p += add;
        if (p > targetP)
            p = targetP;

        _progresstxt.text = (int)(p * 100) + "%";
        //_bar.localScale = new Vector3(p, _bar.localScale.y);
        _offsetHelp.x = (1- p) * _width * -1;
        _bar.offsetMax = _offsetHelp;
        if (p >= 1)
        {
            p = 1;
            isComplete = true;
            DispatchEvent(BaseEvent.COMPLETE);
        }

    }

    void Update () {
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _isShow = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Reset();
        _isShow = false;
    }

    public void OnEnable()
    {
        //txt.text = "0%";
        Reset();
    }

    public void Reset()
    {
        _offsetHelp.x = -_width;
        _bar.offsetMax = _offsetHelp;
        isComplete = false;
        p = 0;
        _progresstxt.text = "0%";
        _titleTxt.text = "正在进入游戏...";
        add = SPEED;
    }

    public bool HasEventListener(string type)
    {
        return _dispatcher.HasEventListener(type);
    }

    public void AddEventListener(string type, Action<BaseEvent> listener)
    {
        _dispatcher.AddEventListener(type, listener);
    }

    public void RemoveAllEventListender()
    {
        _dispatcher.RemoveAllEventListender();
    }

    public void DispatchEvent(string type, object eventObj = null)
    {
        _dispatcher.DispatchEvent(type, eventObj);
    }

    public void RemoveEventListener(string type, Action<BaseEvent> listener)
    {
        _dispatcher.RemoveEventListener(type, listener);
    }
    private EventDispatcher _dispatcher;
    private static SceneLoadingPanel _instance;
}
