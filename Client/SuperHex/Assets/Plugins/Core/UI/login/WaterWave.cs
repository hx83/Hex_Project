using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Image))]
public class WaterWave : MonoBehaviour {

    [Serializable]
    public class Value {
        public float start;
        public float end;
    }

    public float minLifeTime = 3f;
    public float maxLifeTime = 5f;
    private float _lifeTime = 1.5f;

    public Value fadeValue;
    public AnimationCurve fadeCurve;
    public Value scaleValue;
    public AnimationCurve scaleCurve;

    private float _currentLifeTime = 0;
    private Image _image;

    public bool startp = false;

	// Use this for initialization
	void Awake () {
	    _lifeTime = minLifeTime + (maxLifeTime - minLifeTime)*Random.value;
        Destroy(gameObject, _lifeTime);
	    _image = GetComponent<Image>();
	    _image.color = withAlpha(_image.color, fadeValue.start);
	    transform.localScale = Vector3.one*scaleValue.start;
	}
	
	// Update is called once per frame
	void Update () {
	    _currentLifeTime += Time.deltaTime;
	    var delta = _currentLifeTime/_lifeTime;
	    _image.color = withAlpha(_image.color
	        , valueEvaluate(fadeValue, fadeCurve.Evaluate(delta)));
	    transform.localScale = Vector3.one
            * valueEvaluate(scaleValue, scaleCurve.Evaluate(delta));
    }

    public static float valueEvaluate(Value v, float t) {
        return v.start + (v.end - v.start)*t;
    }

    public static Color withAlpha(Color c, float newAlpha) {
        return new Color(c.r, c.g, c.b, newAlpha);
    }

}
