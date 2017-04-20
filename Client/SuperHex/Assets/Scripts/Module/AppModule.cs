using UnityEngine;

public class AppModule : MonoBehaviour, IAppModule
{
    private ModuleType type;
    protected Transform mask;
    protected object data;
    private bool isHide;

    void Awake()
    {
        OnInit();
    }

    public void Show(object obj = null)
    {
        data = obj;
        this.transform.SetParent(LayerManager.uiLayer, false);
        (transform as RectTransform).anchoredPosition = Vector2.zero;
        isHide = false;
        OnShow();
    }

    public void SetSiblingIndex(int value)
    {
        transform.SetSiblingIndex(value);
    }


    protected virtual void OnDispose()
    {
    }

    public void Destroy()
    {
        OnDispose();
        GameObject.Destroy(gameObject);
    }

    public virtual void Hide()
    {
        (transform as RectTransform).anchoredPosition = GameObjectUtil.HIDE_POS;
        isHide = true;
        OnHide();
    }

    public ModuleType Type
    {
        get { return type; }
        set { type = value; }
    }

    protected virtual void OnInit()
    {
    }

    protected virtual void OnShow()
    {
    }

    protected virtual void OnHide()
    {
    }

    public bool IsHide
    {
        get { return isHide; }
        set { isHide = value; }
    }
}