using UnityEngine;
using UnityEngine.UI;

public class ExitAlertUI : MonoBehaviour
{
    private Button _returnBtn;
    private Button _confirmBtn;
    private Button _joinQQGroupBtn;
    private Text _titleTxt;
    private Text _returnTxt;
    public void Awake()
    {
        _confirmBtn = transform.FindChild("Btns/BtnExit").GetComponent<Button>();
        _confirmBtn.onClick.AddListener(OnExitGame);
        _returnBtn = transform.FindChild("Btns/BtnBack").GetComponent<Button>();
        _returnBtn.onClick.AddListener(OnReturnGame);
        _returnTxt = _returnBtn.transform.FindChild("text").GetComponent<Text>();
        _joinQQGroupBtn = transform.FindChild("BtnJoin").GetComponent<Button>();
        _joinQQGroupBtn.onClick.AddListener(OnAddQQGroup);
        _titleTxt = transform.FindChild("TxtTitle").GetComponent<Text>();
    }

    public void Show(bool isExit = false)
    {
        gameObject.SetActive(true);
        if(isExit)
        {
            _titleTxt.text = "退出游戏";
            _returnTxt.text = "返回";
            _confirmBtn.gameObject.SetActive(true);
        }
        else
        {
            _titleTxt.text = "联系我们";
            _returnTxt.text = "我知道了";
            _confirmBtn.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnAddQQGroup()
    {
        //if (Initiator.instance.Channel == "a4399")
        //    return;
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        bool b = jo.Call<bool>("JoinQQGroup");
        if(!b)
        {
            PopManager.ShowSimpleItem("未安装手机QQ或安装的版本不支持");
        }
#elif UNITY_IPHONE
#endif
    }

    private void OnExitGame()
    {
        Debuger.Log("退出游戏...");
        Application.Quit();
    }

    private void OnReturnGame()
    {
        Hide();
    }
}
