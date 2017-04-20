using UnityEngine;
using System.Collections;

public class LionIconBar : MonoBehaviour {

    public RectTransform thePosition;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    transform.position = thePosition.position;
	}
}
