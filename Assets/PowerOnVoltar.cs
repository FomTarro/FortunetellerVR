using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VRStandardAssets.Utils;

public class PowerOnVoltar : MonoBehaviour {

    [SerializeField]
    Image powerOnTimer;
    [SerializeField]
    Color turnOnColor;

    VRInteractiveItem interactive;
    bool isOver = false;
    bool turnOn = false;


	// Use this for initialization
	void Start () {
        interactive = GetComponent<VRInteractiveItem>();
        interactive.OnOver += StartPowerOn;
        interactive.OnOut += EndPowerOn;
	}
	
    void Update()
    {
        if (turnOn)
        {
            //turn self off
            StartCoroutine(shrink());
            this.enabled = false;
        }

        if (isOver && powerOnTimer.fillAmount < 1)
        {
            powerOnTimer.fillAmount += 0.01f;
        }
        else if (isOver && powerOnTimer.fillAmount >= 1)
        {
            //turn zoltar on
            turnOn = true;
            powerOnTimer.color = turnOnColor;
        }
        else if(!isOver && !turnOn && powerOnTimer.fillAmount > 0)
        {
            powerOnTimer.fillAmount -= 0.01f;
        }
    }

    void StartPowerOn()
    {
        isOver = true;
    }

    void EndPowerOn()
    {
        isOver = false;
    }

    private IEnumerator shrink()
    {
        RectTransform timerTransform = powerOnTimer.rectTransform;
        while(timerTransform.sizeDelta.x > 0)
        {
            timerTransform.sizeDelta -= new Vector2(0.05f, 0.05f);
            yield return null;
        }
        GameStateManager gsm = FindObjectOfType<GameStateManager>();
        gsm.ChangeState(gsm.gameObject.AddComponent<ZoltarAwake>());
    }
}
