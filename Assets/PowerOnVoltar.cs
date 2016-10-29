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

    private Vector2 originalSize;

	// Use this for initialization
	void Awake () {
        interactive = GetComponent<VRInteractiveItem>();
        interactive.OnOver += StartPowerOn;
        interactive.OnOut += EndPowerOn;
        originalSize = powerOnTimer.rectTransform.sizeDelta;
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
            powerOnTimer.fillAmount += 0.02f;
        }
        else if (isOver && powerOnTimer.fillAmount >= 1)
        {
            //turn zoltar on
            turnOn = true;
            powerOnTimer.color = turnOnColor;
        }
        else if(!isOver && !turnOn && powerOnTimer.fillAmount > 0)
        {
            powerOnTimer.fillAmount -= 0.02f;
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

    public void ResetVoltar()
    {
        powerOnTimer.fillAmount = 0;
        powerOnTimer.color = Color.white;
        powerOnTimer.rectTransform.sizeDelta = originalSize;
        isOver = false;
        turnOn = false;
        this.enabled = true;
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
