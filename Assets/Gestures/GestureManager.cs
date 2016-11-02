using UnityEngine;
using System.Collections.Generic;

public enum GestureType
{
    Nod,
    Shake,
}

public class GestureManager : MonoBehaviour
{
    [SerializeField]
    private Camera _cameraToMeasure;
    /// <summary>
    /// The head tracking camera from which to obtain gesture data.
    /// </summary>
    public Camera CameraToMeasure
    {
        set { _cameraToMeasure = value; }
    }

    #region Threshold Parameters

    [SerializeField]
    [Tooltip("The minimum difference in quaternion readings to trigger a gesture check.")]
    private float _deltaThreshold = 0.015f;
    /// <summary>
    /// The minimum difference in quaternion readings to trigger a gesture check.
    /// </summary>
    public float DeltaThreshold
    {
        get { return _deltaThreshold; }
        set { _deltaThreshold = value; }
    }

    [SerializeField]
    [Tooltip("The minimum amount of time that each gesture motion must be from the previous to be read as a gesture.")]
    private float _timeThreshold = 1.0f;
    /// <summary>
    /// The minimum amount of time that each gesture motion must be from the previous to be read as a gesture.
    /// </summary>
    public float TimeThreshold
    {
        get { return _timeThreshold; }
        set { _timeThreshold = value; }
    }

    [SerializeField]
    [Tooltip("The minimum amout of passing gesture checks that must occur to create a valid gesture.")]
    private int _gestureAmountThreshold = 3;
    /// <summary>
    /// The minimum amout of passing gesture checks that must occur to create a valid gesture.
    /// </summary>
    public int GestureAmountThreshold
    {
        get { return _gestureAmountThreshold; }
        set { _gestureAmountThreshold = value; }
    }

    #endregion

    private static GestureManager _instance;
    /// <summary>
    /// The instance of the GestureManager in the game.
    /// </summary>
    public static GestureManager Instance
    {
        get { return _instance; }
    }

    private List<GestureListener> _listeners = new List<GestureListener>();
    /// <summary>
    /// The list of all currently existing Gesture Listeners. 
    /// <para>
    /// Listeners are automatically added when created, and are automatically removed when destroyed.
    /// </para>
    /// </summary>
    public List<GestureListener> ListenerRegistry
    {
        get { return _listeners; }
    }

    private Gesture _yes = new Gesture(GestureType.Nod), _no = new Gesture(GestureType.Shake);

    private class Gesture
    {
        float _lastVelocity = 0.0f;
        bool _negDirection = false;
        int _headDirectionChanges = 0;
        float _timeSinceLast = 0.0f;
        float _lastMeasurement = 0.0f;
        GestureType _gestureType;
        public GestureType GestureType
        {
            get { return _gestureType; }
        }

        public Gesture(GestureType gestureType)
        {
            _gestureType = gestureType;
        }

        /// <summary>
        /// Checks to see if this gestur has been completed.
        /// </summary>
        /// <param name="newMeasurement">The new measurement data to compare against the previous measurement data.</param>
        /// <param name="deltaThreshold">The threshold for difference in measurement. Any difference less than this is considered noise.</param>
        /// <param name="timeThreshold">The minimum amount of time that each gesture motion must be from the previous to be read as a gesture.</param>
        /// <param name="gestureAmountThreshold">The minimum amout of passing gesture checks that must occur to create a valid gesture.</param>
        /// <returns></returns>
        public bool CheckGesture(float newMeasurement, float deltaThreshold, float timeThreshold, int gestureAmountThreshold)
        {
            bool completedGesture = false;

            float newVelocity = (newMeasurement - _lastMeasurement) / Time.fixedDeltaTime;
            if (Mathf.Abs(newMeasurement - _lastMeasurement) > deltaThreshold)
            {
                if ((_lastVelocity < 0 && newVelocity < 0 && !_negDirection) || (_lastVelocity > 0 && newVelocity > 0 && _negDirection))
                {
                    if (_timeSinceLast < timeThreshold)
                    {
                        _headDirectionChanges++;
                        if (_headDirectionChanges >= gestureAmountThreshold)
                        {
                            _headDirectionChanges = 0;
                            completedGesture = true;
                        }
                    }
                    else
                    {
                        _headDirectionChanges = 0;
                        completedGesture = false;
                        
                    }
                    _timeSinceLast = 0.0f;
                    _negDirection = (_lastVelocity < 0 && newVelocity < 0);
                }
            }

            _lastMeasurement = newMeasurement;
            _lastVelocity = newVelocity;
            _timeSinceLast += Time.deltaTime;

            return completedGesture;
        }

    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    void FixedUpdate()
    {
        if(_yes.CheckGesture(_cameraToMeasure.transform.rotation.x, _deltaThreshold, _timeThreshold, _gestureAmountThreshold))
            SendGestureEvent(_yes.GestureType);

        if (_no.CheckGesture(_cameraToMeasure.transform.rotation.y, _deltaThreshold, _timeThreshold, _gestureAmountThreshold))
            SendGestureEvent(_no.GestureType);
    }

    /// <summary>
    /// Sends a Gesture Event of the given type out to all listener objects.
    /// </summary>
    /// <param name="gestureType">The gesture type to send out.</param>
    void SendGestureEvent(GestureType gestureType)
    {
        foreach (GestureListener gl in _listeners)
        {
            foreach (GestureEventCallback ec in gl.EventCallbacks)
            {
                if (ec.ListeningFor == gestureType && ec.IsEnabled)
                {
                    ec.Event.Invoke();
                }
            }
        }
    }

}

