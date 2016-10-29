using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour {

    [SerializeField]
    Animator _zoltarAnimator;

    [SerializeField]
    public Text textField;

    private static GameStateManager _instance;

    private State currentState;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else
        {
            Destroy(this);
        }
    }
    // Use this for initialization
    void Start () {
        textField.text = "";
        ChangeState(gameObject.AddComponent<ZoltarAsleep>());
	}
	
	// Update is called once per frame
	void Update () {
        currentState.UpdateState();
	}

    public void ChangeState(State newState)
    {
        newState.text = textField;
        newState.anim = _zoltarAnimator;
        newState.TriggerEnterState();
        if (currentState != null)
        {
            currentState.TriggerExitState();
            Destroy(gameObject.GetComponent(currentState.label));
        }
        currentState = newState;
    }

    public void RespondNo()
    {
        if (currentState.label == "ZoltarAskFortune")
        {
            textField.text = "'No.'";
            StartCoroutine(Wait(2, "No"));
            
        }
    }

    public void RespondYes()
    {
        if(currentState.label == "ZoltarAskFortune")
        {
            textField.text = "'Yes.'";
            StartCoroutine(Wait(2, "Yes"));
            
        }
    }

    private IEnumerator Wait(int seconds, string res)
    {
        yield return new WaitForSeconds(seconds);
        if (res == "No")
        {
            textField.text = "Ok.";
            ChangeState(gameObject.AddComponent<ZoltarAsleep>());
        }
        else if (res == "Yes")
            ChangeState(gameObject.AddComponent<ZoltarCreateFortune>());
    }
}

public abstract class State : MonoBehaviour
{
    public string label { get; set; }
    public Text text { get; set; }
    public Animator anim { get; set; }
    public virtual void TriggerEnterState() { }
    public virtual void TriggerExitState() { }
    public virtual void UpdateState() { }
    public virtual void ChangeContext(string new_context) { }
    public string[] animList = new string[]{"Spin", "HeadShake"};
}

public class ZoltarAsleep : State
{
    public ZoltarAsleep() { label = "ZoltarAsleep"; }
    public override void TriggerEnterState()
    {
        text.text = "";
        StartCoroutine(BackToSleep());
        //Zoltar in sleeping animation
    }

    private IEnumerator BackToSleep()
    {
        FindObjectOfType<CabinetLight>().LightDown();
        yield return new WaitForSeconds(1f);
        anim.SetInteger("State", 0);
        FindObjectOfType<PowerOnVoltar>().ResetVoltar();
    }

    public override void TriggerExitState()
    {
        //
    }
}

public class ZoltarAwake : State
{

    public ZoltarAwake() { label = "ZoltarAwake"; }

    public override void TriggerEnterState()
    {
        anim.SetInteger("State", 1);
        text.StartCoroutine(ZoltarWaitAndChange());
    }

    private IEnumerator ZoltarWaitAndChange()
    {
        FindObjectOfType<CabinetLight>().LightUp();
        yield return new WaitForSeconds(3);
        text.text = "WHO WAKES GALDOR?";
        yield return new WaitForSeconds(2);
        FindObjectOfType<GameStateManager>().ChangeState(gameObject.AddComponent<ZoltarAskFortune>());
    }
}

public class ZoltarAskFortune : State
{
    public ZoltarAskFortune() { label = "ZoltarAskFortune"; }
    public int askCount = -1;
    public override void TriggerEnterState()
    {
        askCount++;
        List<EventCallback> list = FindObjectOfType<GestureListener>().EventCallbacks;
        foreach (EventCallback cb in list)
            cb.enabled = true;
        if (askCount > 0)
            text.text = "Would you like another fortune?";
        else
            text.text = "Would you like a fortune?";
    }

    public override void TriggerExitState()
    {
        List<EventCallback> list = FindObjectOfType<GestureListener>().EventCallbacks;
        foreach (EventCallback cb in list)
            cb.enabled = false;
    }
}

public class ZoltarCreateFortune : State
{
    public ZoltarCreateFortune() { label = "ZoltarCreateFortune"; }
    public override void TriggerEnterState()
    {
        anim.SetTrigger("Spin");
        text.text = FindObjectOfType<TextGenerator>().GenerateRandom();
        Debug.Log(text.text);
        StartCoroutine(WaitAndChange(5, "..."));
    }

    private IEnumerator WaitAndChange(int seconds, string change_to)
    {
        yield return new WaitForSeconds(seconds);
        if(change_to == "...")
        {
            text.text = "...";
            StartCoroutine(WaitAndChange(2, ""));
        }
        else
        {
            FindObjectOfType<GameStateManager>().ChangeState(gameObject.AddComponent <ZoltarAskFortune>());
        }
    }
}

