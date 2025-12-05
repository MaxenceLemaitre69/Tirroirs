using System;
using UnityEngine;

namespace Plat.Gameplay
{
    public class PlayerMovement : MonoBehaviour, IPlayerComponent
    {
        public PlayerController Controller { get; set; }
        
        public Vector2 Direction { get; private set; }
        public Vector2 TargetVelocity { get; private set; }
        public Vector2 CurrentVelocity { get; private set; }

        [SerializeField] private Rigidbody2D rb2d;
        
        [SerializeField] private float maxSpeed;
        [SerializeField] private float directionAlignDamping;
        
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        
        
        private void Update()
        {
            ComputeDirection();
            ComputeTargetVelocity();
        }

        private void FixedUpdate()
        {
            ApplyVelocity();
        }

        private void ComputeDirection()
        {
            Vector2 direction = Controller.PlayerControls.MoveInput;
            Direction = direction.normalized;
        }

        private void ComputeTargetVelocity()
        {
            Vector2 lastTargetVelocity = TargetVelocity;
            
            bool wantsToStop = Direction.sqrMagnitude < 0.1f;
            float finalTargetSpeed = wantsToStop ? 0 : maxSpeed;
            
            Vector2 finalTargetVelocity = Direction * finalTargetSpeed;

            Vector2 targetDirection = Vector2.Lerp(
                lastTargetVelocity, 
                finalTargetVelocity, 
                directionAlignDamping * Time.deltaTime).normalized;

            float lastTargetSpeed = TargetVelocity.magnitude;
            float delta = wantsToStop ? -deceleration : acceleration;
            
            //Ajout de l'acceleration en fonction du temps en seconde
            float targetSpeed = lastTargetSpeed + delta * Time.deltaTime;
            targetSpeed = Mathf.Clamp(targetSpeed, 0, maxSpeed);
            
            TargetVelocity = targetDirection * targetSpeed;
        }

        private void ApplyVelocity()
        {
            CurrentVelocity = TargetVelocity;
            
            rb2d.linearVelocity = CurrentVelocity;
        }
    }
}