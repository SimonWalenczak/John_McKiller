using UnityEngine;

public class ShipController : MonoBehaviour
{
    public static ShipController Instance;

    [Header("State")] public bool IsNavigate;
    public GameObject PlayerPrefab;

    [Header("References")] WindController wc;

    [SerializeField] Transform[] Sails;
    [SerializeField] GameObject[] SailsClosed;
    [SerializeField] Transform[] SailsSupport;
    [SerializeField] private Transform _boatPivot;

    [Space(10)] [Header("----Controls----")] [Header("--Boat Turn--")] [SerializeField]
    private float _boatTurnSpeedMax;

    [SerializeField] private float _boatTurnSpeedMin;
    [Space(10)] public float CurrentBoatTurnSpeed;
    private float _turnRatio;
    private float _limitTurnRatio;

    [Header("--Sails--")] [SerializeField] private float _sailTurnSpeed;

    [Space(10)] [SerializeField] private float _maxLeftRotation;
    [SerializeField] private float _maxRightRotation = 50f;

    [Space(10)] [SerializeField] private float _raisingSpeed;
    [SerializeField] private float _loweringSpeed;

    [Space(10)] [Header("----Movement----")] [SerializeField]
    private float _movementSpeedMax;

    [Space(10)] [SerializeField] private float _accelerationRatio;
    [SerializeField] private float _decelerationRatio;

    [Space(10)] private float _predictedMovementSpeed;
    [HideInInspector] public float ActualMovementSpeed;
    private float _speedRatio;

    [Space(10)] [SerializeField] private float _limitSpeedBeforeDeceleration;
    [SerializeField] private float _speedBeforeBeingStatic;
    public bool IsStatic;

    [Header("Park")] public bool IsParked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another Ship Controller in this scene !");
        }
    }

    private void Start()
    {
        wc = WindController.Instance;
    }

    private void GetStateNav()
    {
        if (wc.sailRef.localScale.y <= 0.1f)
        {
            IsStatic = true;
            _predictedMovementSpeed = Mathf.Lerp(_predictedMovementSpeed, 0, _decelerationRatio * Time.deltaTime);
        }
        else
        {
            IsStatic = false;
        }
    }

    private void TurningBoat()
    {
        _limitTurnRatio = _boatTurnSpeedMin / _boatTurnSpeedMax;

        _turnRatio = 1 - (ActualMovementSpeed / _movementSpeedMax);
        //TurnRatio = 1 - wc.sailRef.localScale.y;

        if (_turnRatio <= _limitTurnRatio)
            _turnRatio = _limitTurnRatio;

        CurrentBoatTurnSpeed = _boatTurnSpeedMax * _turnRatio;

        if (Input.GetKey(KeyCode.A))
        {
            _boatPivot.Rotate(Vector3.down * CurrentBoatTurnSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            _boatPivot.Rotate(Vector3.up * CurrentBoatTurnSpeed * Time.deltaTime);
        }
    }

    private void TurningSails()
    {
        for (int i = 0; i < SailsSupport.Length; i++)
        {
            Transform supportSail = SailsSupport[i];

            if (Input.GetKey(KeyCode.Q))
            {
                RotateSails(supportSail, -1);
            }

            if (Input.GetKey(KeyCode.E))
            {
                RotateSails(supportSail, 1);
            }
        }
    }

    private void RotateSails(Transform sail, int direction)
    {
        Quaternion currentRotation = sail.localRotation;
        Vector3 currentEulerAngles = currentRotation.eulerAngles;

        // Adjust for 360-degree rotation
        if (currentEulerAngles.y > 180f)
            currentEulerAngles.y -= 360f;

        float targetRotation = currentEulerAngles.y + direction * _sailTurnSpeed * Time.deltaTime;
        targetRotation = Mathf.Clamp(targetRotation, _maxLeftRotation, _maxRightRotation);

        sail.localRotation = Quaternion.Euler(0f, targetRotation, 0f);
    }

    public void RaisingSails()
    {
        for (int i = 0; i < Sails.Length; i++)
        {
            Transform sail = Sails[i];

            sail.gameObject.SetActive(true);
            SailsClosed[i].SetActive(false);

            if (sail.localScale.y > 0.1f)
                sail.localScale = Vector3.Lerp(sail.transform.localScale,
                    new Vector3(0.8f, -2, sail.transform.localScale.z), _raisingSpeed * Time.deltaTime);
            else
            {
                sail.gameObject.SetActive(false);
                SailsClosed[i].SetActive(true);
            }
        }
    }

    public void LoweringSails()
    {
        for (int i = 0; i < Sails.Length; i++)
        {
            Transform sail = Sails[i];
            sail.gameObject.SetActive(true);
            SailsClosed[i].SetActive(false);

            if (sail.localScale.y < 1)
                sail.localScale = Vector3.Lerp(sail.transform.localScale,
                    new Vector3(1.2f, 2, sail.transform.localScale.z), _loweringSpeed * Time.deltaTime);
        }
    }

    private void SetSails()
    {
        if (Input.GetKey(KeyCode.W))
        {
            RaisingSails();
        }

        if (Input.GetKey(KeyCode.S))
        {
            LoweringSails();
        }
    }

    public float GetPredictedSpeed()
    {
        _speedRatio = wc.sailRef.localScale.y * (wc.sailRef.localScale.z / wc.maxScale);

        if (IsStatic == false)
        {
            _predictedMovementSpeed = _movementSpeedMax * _speedRatio;
        }
        else
        {
            _speedRatio = _limitSpeedBeforeDeceleration;
        }

        return _predictedMovementSpeed;
    }

    void Update()
    {
        if (IsNavigate)
        {
            if (ActualMovementSpeed >= _speedBeforeBeingStatic || IsStatic == false)
            {
                ActualMovementSpeed =
                    Mathf.Lerp(ActualMovementSpeed, _predictedMovementSpeed, _accelerationRatio * Time.deltaTime);
            }
            else if (IsStatic)
            {
                ActualMovementSpeed = 0;
            }

            _boatPivot.position += _boatPivot.forward * ActualMovementSpeed * Time.deltaTime;

            GetPredictedSpeed();
            GetStateNav();

            TurningBoat();
            TurningSails();
            SetSails();
        }
    }
}