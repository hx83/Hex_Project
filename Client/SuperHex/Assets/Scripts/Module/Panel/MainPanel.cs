using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainPanel : AppModule 
{
    private Button startBtn;
    protected override void OnInit()
    {
        startBtn = this.transform.FindChild("startBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(StartGame);
    }

    protected override void OnShow()
    {
    }

    protected override void OnHide()
    {
    }

    protected override void OnDispose()
    {
    }


    private void StartGame()
    {
        MainEntry.Instance.GameStart();
        Hide();
    }
}
