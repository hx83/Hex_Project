using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AlertUI : MonoBehaviour
{
    private Vector3 confirmBtnPos;
    public static AlertUI Show(string content, System.Action OnConfirm, EnumAlertStyle style = EnumAlertStyle.CONFIRM_ONLY, System.Action OnCancel = null, string confirmBtnName = "", string cancelBtnName = "")
    {
        GameObject _ui;
        if (_cacheList.Count > 0)
        {
            _ui = _cacheList.Dequeue();
        }
        else
        {
            _ui = Instantiate(_prefab);
            _ui.SetActive(true);
            _ui.AddComponent<AlertUI>();
        }
        RectTransform tranf = _ui.GetComponent<RectTransform>();
        tranf.localScale = new Vector2(0.3f, 0.3f);
        tranf.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        AlertUI _script = _ui.GetComponent<AlertUI>();
        _script.SetStyle(style);
        _script.SetContent(content);
        _script.SetCallBack(OnConfirm, OnCancel);
        _script.SetBtnName(confirmBtnName, cancelBtnName);
        tranf.SetParent(_root, false);
        _ui.SetActive(true);
        return _script;
    }

    public static void Setup()
    {
        _root = LayerManager.topUILayer;
        _cacheList = new Queue<GameObject>();
        //_prefab = _root.FindChild("AlertUI").gameObject;
        _prefab = (GameObject)Resources.Load("Prefabs/Utils/AlertUI");
    }

    void Awake()
    {
        _contentTxt = transform.FindChild("TxtContent").GetComponent<Text>();
        _confirmBtn = transform.FindChild("Btns/BtnConfirm").GetComponent<Button>();
        _cancelBtn = transform.FindChild("Btns/BtnCancel").GetComponent<Button>();
        _confirmBtnTxt = transform.FindChild("Btns/BtnConfirm/Text").GetComponent<Text>();
        _cancelBtnTxt = transform.FindChild("Btns/BtnCancel/Text").GetComponent<Text>();
        _confirmBtn.onClick.AddListener(OnConfirmBtnClicked);
        _cancelBtn.onClick.AddListener(OnCancelBtnClicked);

        confirmBtnPos = _confirmBtn.transform.localPosition;
    }

    public void SetStyle(EnumAlertStyle style)
    {
        if (style == EnumAlertStyle.CONFIRM_ONLY)
        {
            _confirmBtn.gameObject.SetActive(true);
            _cancelBtn.gameObject.SetActive(false);

            _confirmBtn.transform.localPosition = new Vector3(0, confirmBtnPos.y, confirmBtnPos.z);
        }
        else if(style == EnumAlertStyle.CONFIRM_CANCEL)
        {
            _confirmBtn.gameObject.SetActive(true);
            _cancelBtn.gameObject.SetActive(true);
            _confirmBtn.transform.localPosition = confirmBtnPos;
        }
    }

    public void SetContent(string content)
    {
        _contentTxt.text = content;
    }

    public void SetCallBack(System.Action OnConfirm, System.Action OnCancel = null)
    {
        _onConfirm = OnConfirm;
        _onCancel = OnCancel;
    }

    public void SetBtnName(string confirmBtnName = "", string cancelBtnName = "")
    {
        if (confirmBtnName != "")
            _confirmBtnTxt.text = confirmBtnName;
        if (cancelBtnName != "")
            _cancelBtnTxt.text = cancelBtnName;
    }

    private void OnConfirmBtnClicked()
    {
        if (_onConfirm != null)
            _onConfirm.Invoke();
        Hide();
    }

    private void OnCancelBtnClicked()
    {
        if (_onCancel != null)
            _onCancel.Invoke();
        Hide();
    }

    public void Hide()
    {
        _onConfirm = null;
        _onCancel = null;
        //_titleTxt.text = "";
        _contentTxt.text = "";
        _confirmBtnTxt.text = "确定";
        _cancelBtnTxt.text = "取消";
        gameObject.SetActive(false);
        _cacheList.Enqueue(gameObject);
    }

    private Text _contentTxt;
    private Button _confirmBtn;
    private Text _confirmBtnTxt;
    private Button _cancelBtn;
    private Text _cancelBtnTxt;
    private System.Action _onConfirm;
    private System.Action _onCancel;
    private static Transform _root;
    private static Queue<GameObject> _cacheList;
    private static GameObject _prefab;
}

public enum EnumAlertStyle
{
    CONFIRM_ONLY,
    CONFIRM_CANCEL,
}
