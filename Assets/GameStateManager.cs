using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameStateManager : MonoBehaviour{

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
        ChangeState(new ZoltarAsleep());
	}
	
	// Update is called once per frame
	void Update () {
        currentState.UpdateState();
	}

    public void ChangeState(State newState)
    {
        newState.TriggerEnterState();
        currentState.TriggerExitState();
        currentState = newState;
        currentState.text = textField;
    }
}

public abstract class State
{
    public string label { get; set; }
    public Text text { get; set; }
    public virtual void TriggerEnterState() { }
    public virtual void TriggerExitState() { }
    public virtual void UpdateState() { }
    public virtual void ChangeContext(string new_context) { }
}

public class ZoltarAsleep : State
{
    public ZoltarAsleep() { label = "ZoltarAsleep"; }
    public override void TriggerEnterState()
    {
        //Zoltar in sleeping animation
    }
    public override void TriggerExitState()
    {
        //
    }
}

public class ZoltarActive : State
{
    public ZoltarActive() { label = "ZoltarActive"; }
    public override void TriggerEnterState()
    {
        text.text = "WHO WAKES ZOLTAR?";
    }
    public override void TriggerExitState()
    {
        //have zoltar go back to sleep

    }
    public override void ChangeContext(string new_context)
    {
        
    }
}