using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class GestureEventCallback
{
    public GestureEventCallback(string label, GestureType gestureType, bool isEnabled, GestureEvent gEvent)
    {
        callbackLabel = label;
        gestureListeningFor = gestureType;
        enabled = isEnabled;
        onEvent = gEvent;
    }

    [Tooltip("The identifier for this callback, used for looking it up later. Should be unique.")]
    [SerializeField]
    private string callbackLabel;
    /// <summary>
    /// The identifier for this callback, used for looking it up later. Should be unique.
    /// </summary>
    public string Label
    {
        get { return callbackLabel; }
    }

    [Tooltip("The type of gesture that this callback is listening for.")]
    [SerializeField]
    private GestureType gestureListeningFor;
    /// <summary>
    /// The type of gesture that this callback is listening for.
    /// </summary>
    public GestureType ListeningFor
    {
        get { return gestureListeningFor; }
    }

    [Tooltip("Is this callback currently enabled?")]
    [SerializeField]
    private bool enabled = true;
    /// <summary>
    /// Is this callback currently enabled?
    /// </summary>
    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    [Tooltip("The event to invoke upon a gesture.")]
    [SerializeField]
    private GestureEvent onEvent = new GestureEvent();
    /// <summary>
    /// The event to invoke upon a gesture.
    /// </summary>
    public GestureEvent Event
    {
        get { return onEvent; }
    }
}

[System.Serializable]
public class GestureEvent : UnityEvent { }

/// <summary>
/// The listener component. Simply place this on an object to have it listen for head gestures.
/// </summary>
public class GestureListener : MonoBehaviour {

    [SerializeField]
    private List<GestureEventCallback> _eventCallbacks = new List<GestureEventCallback>();
    public List<GestureEventCallback> EventCallbacks
    {
        get { return _eventCallbacks; }
    }

    void Start()
    {
        if (GestureManager.Instance != null)
            GestureManager.Instance.ListenerRegistry.Add(this);
    }

    void OnDestroy()
    {
        if(GestureManager.Instance != null)
            GestureManager.Instance.ListenerRegistry.Remove(this);
    }

    /// <summary>
    /// Add a given callback to the list.
    /// </summary>
    /// <param name="callback"></param>
    public void AddCallback(GestureEventCallback callback)
    {
        _eventCallbacks.Add(callback);
    }

    /// <summary>
    /// Remove a callback with the given label from the list.
    /// </summary>
    /// <param name="label">The label to search for.</param>
    public void RemoveCallback(string label)
    {
        _eventCallbacks.Remove(FindCallback(label));
    }

    /// <summary>
    /// Searches through the list of callbacks on this listener for the first instance of a callback with the supplied label. Returns null if no such callback exists on the listener.
    /// </summary>
    /// <param name="label">The label to search for.</param>
    /// <returns></returns>
    public GestureEventCallback FindCallback(string label)
    {
        foreach(GestureEventCallback ec in _eventCallbacks)
        {
            if (ec.Label.Equals(label))
            {
                return ec;
            }
        }
        return null;
    }
}
