using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Game;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private EngineConfiguration _engineConfiguration;

    public float MaxSlope;
    public float DynamicFriction;

    private CharacterController _controller;

    private Engine _engine;
    

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _engine = new Engine(_controller,transform, _engineConfiguration);

    }

    // Update is called once per frame
    void Update()
    {
        _engine.Begin();
        

        var camera = Camera.main;

        //Determine the player surface
        var groundNormal = Vector3.up;
        var groundPoint = Vector3.zero;

        var characterCenter = transform.position + _controller.center;
        var characterRayCastHeight = _controller.height / 2 + _controller.skinWidth * 2;

        var groundLayer = LayerMask.GetMask("Walkable");
        float radius = _controller.radius * 0.99f;

        //determine point of floor contact
        RaycastHit hitInfo;
        var groundPointRay = new Ray(characterCenter, Vector3.down);
        if (Physics.SphereCast(groundPointRay, radius, out hitInfo, characterRayCastHeight, groundLayer))
        { 
            groundPoint = hitInfo.point;
        }

        //determine orientation of surface at floor contact
        var groundNormalRay = new Ray(new Vector3(groundPoint.x, characterCenter.y, groundPoint.z), Vector3.down);
        if (Physics.Raycast(groundNormalRay, out hitInfo, characterRayCastHeight))
        {
            groundNormal = hitInfo.normal;
        }

        //determine camera forward and right in xz-plane
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;

      

        //get left stick input
        float horizontalLeft = UnityEngine.Input.GetAxis("LeftStick_Horizontal");
        float verticalLeft = UnityEngine.Input.GetAxis("LeftStick_Vertical");

        // calculate the player movement direction
        Vector2 directionLeft = new Vector2(horizontalLeft, verticalLeft);
        directionLeft = directionLeft.sqrMagnitude > 2 ? directionLeft.normalized : directionLeft;
        Vector3 worldDirection = cameraRight * directionLeft.x + cameraForward * directionLeft.y;

        if (_controller.isGrounded)
        {
            _engine.ApplyGround(groundNormal);

            _engine.ApplyMovement(groundNormal, worldDirection);          
           
        }

        _engine.Commit();
    }
}
