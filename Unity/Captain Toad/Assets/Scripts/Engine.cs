using UnityEngine;
using System;

namespace Game
{
    [Serializable]
    public class EngineConfiguration
    {
        public float Speed;
        public float FallSpeed;
    }

    public class Engine
    {
        private readonly CharacterController _controller;

        private Transform _transform;
        private Vector3 _velocity;
        private EngineConfiguration _engineConfiguration;

        public Engine(CharacterController controller, Transform transform, EngineConfiguration engineConfiguration)
        {
            _controller = controller;
            _engineConfiguration = engineConfiguration;
            _transform = transform;
        }

        public void Begin()
        {
            _velocity = _controller.velocity;
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            _velocity += Physics.gravity * Time.deltaTime * _engineConfiguration.FallSpeed;
        }

        public void ApplyGround(Vector3 groundNormal)
        {
            //only keep vertical velocity
            _velocity = Vector3.Scale(_velocity, new Vector3(0, 1, 0));

            //transform velocity into pure velocity along plane.
            _velocity = Vector3.ProjectOnPlane(_velocity, groundNormal);
        }

        public void ApplyMovement(Vector3 groundNormal, Vector3 worldDirection)
        {
            //calculate ground orientation
            Vector3 groundTangent = worldDirection;
            Vector3.OrthoNormalize(ref groundNormal, ref groundTangent);

            //add movement to velocity
            _velocity += groundTangent * worldDirection.magnitude * _engineConfiguration.Speed;
        }
        public void Commit()
        {
            _controller.Move(_velocity * Time.deltaTime);
        }       

        
    }
}
