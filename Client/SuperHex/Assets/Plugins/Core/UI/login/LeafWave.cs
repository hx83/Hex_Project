using UnityEngine;
using Sa1.LoopEase;

public class LeafWave : MonoBehaviour {

    public float shakeTime = 1f;
    public float scaleTime = 1f;

    public Transform transRoot;
    public Transform trans0;
    public Transform trans1;

    private LoopEase _shakeEase;
    private LoopEase _scaleEase;
    private float _timeScale;

    private Vector3 _startDirection;
    private Vector3 _endDirection;
    private Vector3 _startScale;

    void Awake() {
        _shakeEase = LoopEase.sin().init(0, 1)
            .init(SinEase.EaseType.Rondom);
	    _scaleEase = LoopEase.sin().init(0, 1)
            .init(SinEase.EaseType.Rondom);

	    _timeScale = shakeTime + Random.value * 0.2f; // todo: set to config
        _startScale = transRoot.localScale;
	    _startDirection = transRoot.up;
	    _endDirection = Quaternion.FromToRotation(
	        trans0.position - transRoot.position
            , trans1.position - transRoot.position)
            * transRoot.up;
    }

	void Start () {

    }

    void Update () {
        var deltaShake = _shakeEase.update(Time.deltaTime*_timeScale);
        transRoot.up = Vector3.Slerp(_startDirection, _endDirection, deltaShake);

        var deltaScale = _scaleEase.update(Time.deltaTime*scaleTime);
        transRoot.localScale = (0.1f*deltaScale + 0.9f)*_startScale;
    }

    void OnDrawGizmos() {
        drawACross(transRoot.position);
        drawACross(trans0.position);
        drawACross(trans1.position);
        Gizmos.DrawLine(transRoot.position, trans0.position);
        Gizmos.DrawLine(transRoot.position, trans1.position);
    }

    public static void drawACross(Vector3 point) {
        Gizmos.DrawLine(point + (Vector3.up * 0.20f), point + (Vector3.down * 0.20f));
        Gizmos.DrawLine(point + (Vector3.right * 0.20f), point + (Vector3.left * 0.20f));
    }
}
