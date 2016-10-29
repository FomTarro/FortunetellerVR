using UnityEngine;
using System.Collections;

public class FadeLight : MonoBehaviour {

    Light light;

    [SerializeField]
    float maxIntensity;
    bool fadeIn = true;

	// Use this for initialization
	void Start () {
        light = GetComponent<Light>();
        light.intensity = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (fadeIn)
        {
            if (light.intensity > maxIntensity)
                fadeIn = false;
            light.intensity += 0.005f;
        } else
        {
            if (light.intensity <= 0)
                fadeIn = true;
            light.intensity -= 0.005f;
        }
	}
}
