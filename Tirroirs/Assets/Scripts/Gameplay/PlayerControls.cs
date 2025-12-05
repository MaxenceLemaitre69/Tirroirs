using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Plat.Gameplay
{
    public class PlayerControls : MonoBehaviour, IPlayerComponent
    {
        public PlayerController Controller { get; set; }
        public Vector2 MoveInput { get; private set; }
        
        public bool WantsToJump { get; private set; }

        private void Update()
        {
            //A rework plus tard
            MoveInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();

            WantsToJump = InputSystem.actions.FindAction("Jump").IsPressed();
        }

    }
}