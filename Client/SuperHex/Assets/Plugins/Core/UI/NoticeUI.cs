using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 游戏前公告
/// </summary>
public class NoticeUI : MonoBehaviour {

	public Text TXTTitle;
	public Text TXTContent;
	public Button CloseBtn;
	public void Online()
	{
		gameObject.SetActive (true);
		TXTTitle = transform.Find ("Bg/Title/TXTTitle").GetComponent<Text>();
		TXTContent = transform.Find ("Mask/TXT").GetComponent<Text>();
		CloseBtn  = transform.Find ("CloseBtn").GetComponent<Button>();
		CloseBtn.onClick.AddListener (CloseBtnOnClick);
		gameObject.SetActive (false);
	}
	public void Open(string _Title, string _Content)
	{
		TXTTitle.text = _Title;
		TXTContent.text = _Content;
		gameObject.SetActive (true);
	}
	public void Close()
	{
		gameObject.SetActive (false);
	}
	public void CloseBtnOnClick()
	{
		this.Close ();
	}
}
