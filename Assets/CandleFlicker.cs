using UnityEngine;
using System.Collections;

public class CandleFlicker : MonoBehaviour {

    private Light _light;

	// Use this for initialization
	void Start () {
        _light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        _light.intensity = 4.0f + Random.Range(-0.5f, 0.5f);
	}
}
