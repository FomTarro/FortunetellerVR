using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class EventCallback
{
    [SerializeField]
    public GestureType gestureListeningFor;
    [SerializeField]
    public bool enabled = true;
    [SerializeField]
    public GestureEvent onEvent = new GestureEvent();
}

[System.Serializable]
public class GestureEvent : UnityEvent { }


public class GestureListener : MonoBehaviour {

    [SerializeField]
    private List<EventCallback> _eventCallbacks = new List<EventCallback>();
    public List<EventCallback> EventCallbacks
    {
        get { return _eventCallbacks; }
    }

    // Use this for initialization
    void Start () {
	
	}
}
