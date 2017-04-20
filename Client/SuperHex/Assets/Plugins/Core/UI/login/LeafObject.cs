using DG.Tweening;
using UnityEngine;

public class LeafObject : MonoBehaviour {

    public float distanceLevel = 0.02f;
    public RectTransform _leaf;

    protected Vector3 _originScale;
    
    protected void Start () {
        _originScale = _leaf.localScale;
    }

    protected void goAway() {
        _leaf.DOKill(true);
        _leaf.DOScale(_originScale, 1.5f);
    }

    protected void goNear() {
        _leaf.DOKill(true);
        _leaf.DOScale((1+distanceLevel) * _originScale, 1.5f);
    }
}