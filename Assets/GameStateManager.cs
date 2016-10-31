using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour {

    [SerializeField]
    Animator _zoltarAnimator;

    [SerializeField]
    int[] _animStateList;

    public static int[] AnimList;

    [SerializeField]
    public SpeechText textField;

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
        AnimList = _animStateList;
        textField.StartCoroutine(textField.SayText(""));
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
            List<EventCallback> list = FindObjectOfType<GestureListener>().EventCallbacks;
            foreach (EventCallback cb in list)
                cb.enabled = false;
            textField.SetText("'No.'");
            StartCoroutine(Wait(2, "No"));
            
        }
    }

    public void RespondYes()
    {
        if(currentState.label == "ZoltarAskFortune")
        {
            List<EventCallback> list = FindObjectOfType<GestureListener>().EventCallbacks;
            foreach (EventCallback cb in list)
                cb.enabled = false;
            textField.SetText("'Yes.'");
            StartCoroutine(Wait(2, "Yes"));
            
        }
    }

    private IEnumerator Wait(int seconds, string res)
    {
        yield return new WaitForSeconds(seconds);
        if (res == "No")
        {
            yield return textField.StartCoroutine(textField.SayText("Very well then..."));
            ChangeState(gameObject.AddComponent<ZoltarAsleep>());
        }
        else if (res == "Yes")
        {
            ChangeState(gameObject.AddComponent<ZoltarCreateFortune>());
        }
    }
}

public abstract class State : MonoBehaviour
{
    public string label { get; set; }
    public SpeechText text { get; set; }
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
        text.StartCoroutine(text.SayText(""));
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
        ZoltarAskFortune.askCount = -1;
        anim.SetInteger("State", 1);
        text.StartCoroutine(ZoltarWaitAndChange());
    }

    private IEnumerator ZoltarWaitAndChange()
    {
        FindObjectOfType<CabinetLight>().LightUp();
        //yield return new WaitForSeconds(3);
        //text.text = "You have awakened Galdor the Great!";
        yield return new WaitForSeconds(3);
        yield return text.StartCoroutine(text.SayText("You have awakened GALDOR the Great!"));
        FindObjectOfType<GameStateManager>().ChangeState(gameObject.AddComponent<ZoltarAskFortune>());
    }
}

public class ZoltarAskFortune : State
{
    public ZoltarAskFortune() { label = "ZoltarAskFortune"; }
    public static int askCount = -1;
    public override void TriggerEnterState()
    {
        askCount++;
        StartCoroutine(AskPlayer());
    }


    private IEnumerator AskPlayer()
    {
        if (askCount > 0)
            yield return text.StartCoroutine(text.SayText("Would you like another fortune?"));
        else
            yield return text.StartCoroutine(text.SayText("You are here seeking a mystical fortune, yes?\nJust nod your head to proceed."));

        List<EventCallback> list = FindObjectOfType<GestureListener>().EventCallbacks;
        foreach (EventCallback cb in list)
            cb.enabled = true;

        yield return null;
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
        //anim.SetTrigger("Spin");
        //Debug.Log(text.text);
        StartCoroutine(MakeFortune());
    }

    private IEnumerator MakeFortune()
    {
        anim.SetInteger("State", GameStateManager.AnimList[UnityEngine.Random.Range(0, GameStateManager.AnimList.Length)]);
        yield return text.StartCoroutine(text.SayText(FindObjectOfType<TextGenerator>().GenerateRandom()));

        /*
        do
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Booth_Default") || !anim.IsInTransition(0))
                anim.SetTrigger(GameStateManager.AnimList[UnityEngine.Random.Range(0, GameStateManager.AnimList.Length)]);

            yield return null;
        } while (!text.FinishedTalking);
        */

        yield return null;
        StartCoroutine(WaitAndChange(5, "..."));
    }

    private IEnumerator WaitAndChange(int seconds, string change_to)
    {
        anim.SetInteger("State", 1);
        yield return new WaitForSeconds(seconds);
        if (change_to == "...")
        {
            text.StartCoroutine(text.SayText("..."));
            StartCoroutine(WaitAndChange(2, ""));
        }
        else
        {
            FindObjectOfType<GameStateManager>().ChangeState(gameObject.AddComponent<ZoltarAskFortune>());
        }
    }
}

