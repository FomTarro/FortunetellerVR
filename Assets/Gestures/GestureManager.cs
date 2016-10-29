using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GestureType
{
    Nod,
    Shake,
}

public class GestureManager : MonoBehaviour
{
    public Text DebugText;

    private float _velX = 0.0f;
    private float _lastXVal = 0.0f;

    private float _velY = 0.0f;
    private float _lastYVal = 0.0f;

    private bool negDirectionShake = false;
    private bool negDirectionNod = false;

    private float _timeSinceLastX = 0.0f;
    private float _timeSinceLastY = 0.0f;

    private int _shakes = 0;
    private int _nods = 0;


    [SerializeField]
    [Tooltip("The minimum difference in quaternion readings to trigger a gesture check.")]
    private float _deltaThreshold = 0.05f;
    [SerializeField]
    [Tooltip("The minimum amount of time that each gesture check must be from the previous to be read as a gesture.")]
    private float _timeThreshold = 1.0f;
    [SerializeField]
    [Tooltip("The minimum amout of passing gesture checks that must occur to create a valid gesture.")]
    private int _gestureAmountThreshold = 3;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // Nod
        float newVelX = (transform.rotation.x - _lastXVal) / Time.fixedDeltaTime;
        if (Mathf.Abs(transform.rotation.x - _lastXVal) > _deltaThreshold)
        {
            // Negative Velocity
            if (_velX < 0 && newVelX < 0)
            {
                if (!negDirectionNod) // If the direction of velocity has changed
                {
                    if (_timeSinceLastX < _timeThreshold)
                    {
                        IncrementNodGesture();
                    }
                    else
                    {
                        _nods = 0;
                    }
                }
                _timeSinceLastX = 0.0f;
                negDirectionNod = true;
            }

            // Positive Velocity
            else if (_velX > 0 && newVelX > 0)
            {
                if (negDirectionNod) // If the direction of velocity has changed
                {
                    if (_timeSinceLastX < _timeThreshold)
                    {
                        IncrementNodGesture();
                    }
                    else
                    {
                        _nods = 0;
                    }
                }
                _timeSinceLastX = 0.0f;
                negDirectionNod = false;
            }
        }

        // Shake

        float newVelY = (transform.rotation.y - _lastYVal) / Time.fixedDeltaTime;
        if (Mathf.Abs(transform.rotation.y - _lastYVal) > _deltaThreshold)
        {
            // Negative Velocity
            if (_velY < 0 && newVelY < 0)
            {
                if (!negDirectionShake) // If the direction of velocity has changed
                {
                    if (_timeSinceLastY < _timeThreshold)
                    {
                        IncrementShakeGesture();
                    }
                    else
                    {
                        _shakes = 0;
                    }
                }
                _timeSinceLastY = 0.0f;
                negDirectionShake = true;
            }

            // Positive Velocity
            else if (_velY > 0 && newVelY > 0)
            {
                if (negDirectionShake) // If the direction of velocity has changed 
                {
                    if (_timeSinceLastY < _timeThreshold)
                    {
                        IncrementShakeGesture();
                    }
                    else
                    {
                        _shakes = 0;
                    }
                }
                _timeSinceLastY = 0.0f;
                negDirectionShake = false;
            }
        }


        _lastXVal = transform.rotation.x;
        _lastYVal = transform.rotation.y;

        _velY = newVelY;
        _velX = newVelX;

        _timeSinceLastX += Time.fixedDeltaTime;
        _timeSinceLastY += Time.fixedDeltaTime;

    }


    /// <summary>
    /// Increments the Shake gesture counter, and, if it exceeds the threshold, sends out a Shake Gesture Event to all listeners.
    /// </summary>
    void IncrementShakeGesture()
    {
        _shakes++;
        if (_shakes >= _gestureAmountThreshold)
        {
            _shakes = 0;
            foreach(GestureListener gl in FindObjectsOfType<GestureListener>())
            {
                foreach(EventCallback ec in gl.EventCallbacks)
                {
                    if(ec.gestureListeningFor == GestureType.Shake && ec.enabled)
                    {
                        ec.onEvent.Invoke();
                    }
                }
            }
        }
    }


    /// <summary>
    /// Increments the Nod gesture counter, and, if it exceeds the threshold, sends out a Nod Gesture Event to all listeners.
    /// </summary>
    void IncrementNodGesture()
    {
        _nods++;
        if (_nods >= _gestureAmountThreshold)
        {
            _nods = 0;
            foreach (GestureListener gl in FindObjectsOfType<GestureListener>())
            {
                foreach (EventCallback ec in gl.EventCallbacks)
                {
                    if (ec.gestureListeningFor == GestureType.Nod && ec.enabled)
                    {
                        ec.onEvent.Invoke();
                    }
                }
            }
        }
    }
}

