using UnityEngine;
using System;

[Serializable]
public class Configuration
{
   
}
public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private float _xSpeed = 50.0f;
    [SerializeField] private float _ySpeed = 30.0f;

    [SerializeField] private float _yMinLimit = 0f;
    [SerializeField] private float _yMaxLimit = 80f;

    [SerializeField] private float _cameraDistance = -40f;
    [SerializeField] private float _fieldOfViewFar = 60f;
    [SerializeField] private float _fieldOfViewCloseOne = 40f;
    [SerializeField] private float _fieldOfViewCloseTwo = 20f;

    [SerializeField] private float _dampingRotation = 5f;
    [SerializeField] private float _dampingPosition = 3f;
    [SerializeField] private float _dampingFOV = 15f;

    private Vector3 _currentPos;
    private Vector3 _lastPos;

    private float _x = 0.0f;
    private float _y = 0.0f;

    private float _rotationDegree = 45f;

    private bool _zoom;


    enum zoom { normal, zoomOnce, zoomTwice };
    zoom currentState = zoom.normal;
    // Use this for initialization
    void Start()
    {
        
        var camera = Camera.main;
        //start rotation
        camera.transform.position = new Vector3(0, 0, _cameraDistance);
        transform.rotation = Quaternion.Euler(45,0,0);
        transform.position = Vector3.zero;

        //start angles
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;


        _zoom = false;

    }
    void LateUpdate()
    {
        

        //get left stick input
        float horizontalRight = UnityEngine.Input.GetAxis("RightStick_Horizontal");
        float verticalRight = UnityEngine.Input.GetAxis("RightStick_Vertical");

        //input * speed
        _x += horizontalRight * _xSpeed * 0.02f;
        _y -= verticalRight * _ySpeed * 0.02f;

        //clamping of angle
        _y = ClampAngle(_y, _yMinLimit, _yMaxLimit);

        Quaternion rotation = Quaternion.Euler(_y, _x, 0);


        //zoom in/out when pressing A
        //if (Input.GetButtonUp("A"))
        //    _zoom = !_zoom;

        DoubleZoom();
        if (Input.GetButtonUp("A"))
        {
            ++currentState;
        }

        // if (_zoom)
        //    CameraZoom(_characterController.transform.position, _fieldOfViewCloseOne);      
        //else
        //    CameraZoom(new Vector3(0, _characterController.transform.position.y, 0), _fieldOfViewFar);

        //rotate 45 degrees around x-axis
        if (Input.GetButtonDown("LB"))
        {
            Rotation45Degrees(rotation, _rotationDegree);
        }

        if (Input.GetButtonDown("RB"))
        {
            Rotation45Degrees(rotation, - _rotationDegree);
        }
    
        //camera rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _dampingRotation );
       
    }
    private void DoubleZoom()
    {       
       
        switch (currentState)
        {
            case zoom.normal:
                //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normal, Time.deltaTime * smooth);
                CameraZoom(new Vector3(0, _characterController.transform.position.y, 0), _fieldOfViewFar);
                
                break;
            case zoom.zoomOnce:
                //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomedOnce, Time.deltaTime * smooth);
                CameraZoom(_characterController.transform.position, _fieldOfViewCloseOne);
                break;
            case zoom.zoomTwice:
                //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomedTwice, Time.deltaTime * smooth);
                CameraZoom(_characterController.transform.position, _fieldOfViewCloseTwo);
                break;
            default:
                currentState = zoom.normal;
                break;
        }

      
    }
    #region CameraZoom
    private void CameraZoom(Vector3 Transform,float FieldOfView)
    {
        var camera = Camera.main;
        transform.position = Vector3.Lerp(transform.position, Transform, Time.deltaTime * _dampingPosition);

        camera.fieldOfView = Mathf.MoveTowards(camera.fieldOfView, FieldOfView, Time.deltaTime * _dampingFOV);
    }
    #endregion

    #region Rotate45Degree
    private void Rotation45Degrees(Quaternion Rotation,float RotationDegree)
    {
        _x += RotationDegree;
        Rotation = Quaternion.Lerp(transform.rotation, Rotation, Time.deltaTime);
    }
    #endregion

    #region ClampAngle
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
    #endregion
}
