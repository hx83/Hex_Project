using UnityEngine;
using System.Collections;

public class WaveSpawnPoint : MonoBehaviour {

    public WaterWave wavePrefab;
    public float spawnInterval;
    public float spawnRange = 1f;
    public float spawnOffsetRange = 2f;

    private float _lastSpawnTime;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    if (Time.time - _lastSpawnTime > spawnInterval) {
	        spawn();
	    }
	}

    public void spawn() {
        _lastSpawnTime = Time.time + Random.value * spawnOffsetRange;
        var t = (GameObject) Instantiate(wavePrefab.gameObject, transform);
        t.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * spawnRange);
    }

    private void OnDrawGizmos() {
        LeafWave.drawACross(transform.position);
    }
}
