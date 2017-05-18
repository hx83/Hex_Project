using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        GameObject go = GameObject.Find("Ball");
        go.SetActive(false);

        float R = 10;

        int n = 50;
        float a = 360f / n;
        Vector3 firstV = new Vector3(3,5,0);

        for (int i = 0; i < n; i++)
        {
            GameObject obj = GameObject.Instantiate(go);
            Vector3 v = Quaternion.AngleAxis(a*i, new Vector3(0, 0, 1)) * firstV * R;
            obj.transform.position = v;
            obj.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
