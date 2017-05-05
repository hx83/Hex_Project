using UnityEngine;
using System.Collections;
using usercmd;
public class NetManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        GlobalBehavior.Setup();

        SocketManager.Instance.AddEventListener(SocketEvent.CONNECT, OnConnect);
        SocketManager.Instance.Connect("127.0.0.1", 9201);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    private void OnConnect(BaseEvent evt)
    {
        Debug.Log("1");
        SocketManager.Send(MsgType.AchievementList);
    }
}
