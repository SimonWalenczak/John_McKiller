using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCam : MonoBehaviour
{
    #region Init.variables
    public Transform target;
    public Camera sceneCamera;

    public float mouseRotateSpeed = 5f;
    public float touchRotateSpeed = 10f;
    public float slerpSmoothValue = 0.3f;

    public float scrollSmoothTime = 0.12f;
    public float editorFOVSensitivity = 5f;
    public float touchFOVSensitivity = 5f;

    public bool canRotate = true;
    private Vector2 swipeDirection;
    private Vector2 touch1OldPos;
    private Vector2 touch2OldPos;
    private Vector2 touch1CurrentPos;
    private Vector2 touch2CurrentPos;
    private Quaternion currentRot;
    private Quaternion targetRot;
    private Touch touch;

    private float rotX;
    private float rotY;

    private float cameraFieldOfView;
    private float cameraFOVDamp;
    private float fovChangeVelocity = 0;
    private float distanceBetweenCameraAndTarget;

    private float minXRotAngle = -85;
    private float maxXRotAngle = 85;
    private float minCameraFieldOfView = 6;
    private float maxCameraFieldOfView = 70;
    Vector3 dir;

    #endregion
    private void Awake()
    {
        GetCameraReference();
    }

    void Start()
    {
        distanceBetweenCameraAndTarget = Vector3.Distance(sceneCamera.transform.position, target.position);
        dir = new Vector3(0, 0, distanceBetweenCameraAndTarget + 2);
        sceneCamera.transform.position = target.position + dir;
        cameraFOVDamp = sceneCamera.fieldOfView;
        cameraFieldOfView = sceneCamera.fieldOfView;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
                #if UNITY_STANDALONE
                Application.Quit();
                #endif

                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
        }

        Cursor.visible = false;

        if (!canRotate)
        {
            return;
        }

        EditorCameraInput();
    }
    private void LateUpdate()
    {
        if (!canRotate)
        {
            return;
        }
        RotateCamera();
        SetCameraFOV();
    }
    public void GetCameraReference()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }
    }

    private void EditorCameraInput()
    {

        if (Input.GetAxis("Mouse X") <= 0 || Input.GetAxis("Mouse X") >= 0)
        {
            rotX += Input.GetAxis("Mouse Y") * mouseRotateSpeed;
            rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed;
            if (rotX < minXRotAngle)
            {
                rotX = minXRotAngle;
            }
            else if (rotX > maxXRotAngle)
            {
                rotX = maxXRotAngle;
            }
        }

        if (Input.mouseScrollDelta.magnitude > 0)
        {
            cameraFieldOfView += Input.mouseScrollDelta.y * editorFOVSensitivity * -1;
        }
    }

    private void RotateCamera()
    {
        Vector3 tempV = new Vector3(rotX, rotY, 0);
        targetRot = Quaternion.Euler(tempV);

        currentRot = Quaternion.Slerp(currentRot, targetRot, Time.smoothDeltaTime * slerpSmoothValue * 50);
        sceneCamera.transform.position = target.position + currentRot * dir;
        sceneCamera.transform.LookAt(target.position);
    }
    void SetCameraFOV()
    {

        if (cameraFieldOfView <= minCameraFieldOfView)
        {
            cameraFieldOfView = minCameraFieldOfView;
        }
        else if (cameraFieldOfView >= maxCameraFieldOfView)
        {
            cameraFieldOfView = maxCameraFieldOfView;
        }
        cameraFOVDamp = Mathf.SmoothDamp(cameraFOVDamp, cameraFieldOfView, ref fovChangeVelocity, scrollSmoothTime);
        sceneCamera.fieldOfView = cameraFOVDamp;
    }
}