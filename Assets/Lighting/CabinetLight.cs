using UnityEngine;
using System.Collections;

public class CabinetLight : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LightUp()
    {
        StartCoroutine(LightFlickerOn());
    }

    public void LightDown()
    {
        StartCoroutine(LightFlickerOff());
    }

    IEnumerator LightFlickerOn()
    {
        GetComponent<Light>().intensity = 8.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 2.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 8.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 2.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 8.0f;

    }

    IEnumerator LightFlickerOff()
    {
        GetComponent<Light>().intensity = 8.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 2.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 8.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 2.0f;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Light>().intensity = 2.0f;

    }
}
