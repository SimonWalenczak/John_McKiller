using UnityEngine;

public class Docker : MonoBehaviour
{
    private ShipController ship;

    [HideInInspector] public bool DocksOccupied;

    [Header("References")] [SerializeField]
    private Transform _parkPoint;

    [SerializeField] private Transform _playerSpawnPoint;

    [Header("Boat")] public bool HasBoat;
    [SerializeField] private GameObject BoatParked;

    [Header("Park Settings")] [SerializeField]
    private bool IsInverse;

    [SerializeField] private float ParkSpeed;

    private void Start()
    {
        ship = ShipController.Instance;
    }

    private void Update()
    {
        Vector3 predictedPosition;
        predictedPosition = new Vector3(_parkPoint.position.x, ship.transform.position.y, _parkPoint.position.z);

        Vector3 predictedRotation;

        if (IsInverse)
            predictedRotation = new Vector3(_parkPoint.eulerAngles.x, _parkPoint.eulerAngles.y + 180,
                _parkPoint.eulerAngles.z);
        else
            predictedRotation =
                new Vector3(_parkPoint.eulerAngles.x, _parkPoint.eulerAngles.y, _parkPoint.eulerAngles.z);


        if (HasBoat && ship.IsParked == false)
        {
            //Boat Postion Park
            ship.transform.position =
                Vector3.Lerp(ship.transform.position, predictedPosition, ParkSpeed * Time.deltaTime);
            //Boat Rotation Park
            ship.transform.eulerAngles = Vector3.Lerp(ship.transform.eulerAngles, predictedRotation,
                ParkSpeed * Time.deltaTime);
            //Sails Raising
            ship.RaisingSails();

            //Switch Controller Baot --> Character
            if (Vector3.Distance(BoatParked.transform.position, predictedPosition) <= 0.5f)
            {
                ship.transform.position = predictedPosition;

                ship.PlayerPrefab.transform.position = _playerSpawnPoint.position;
                ship.PlayerPrefab.transform.rotation = _playerSpawnPoint.rotation;

                ship.IsParked = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DocksOccupied) return;
        ShipController.Instance.IsNavigate = false;
        ShipController.Instance.ActualMovementSpeed = 0;

        BoatParked = other.gameObject;
        HasBoat = true;
    }
}