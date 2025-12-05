using System;
using UnityEngine;

namespace Plat.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerCamera PlayerCamera { get; private set; }
        
        [field: SerializeField]
        public PlayerMovement PlayerMovement { get; private set; }
        
        [field: SerializeField]
        public PlayerControls PlayerControls { get; private set; }

        public IPlayerComponent[] Components { get; private set; }
        private void Awake()
        {
            Components = GetComponentsInChildren<IPlayerComponent>();
            for (int i = 0; i < Components.Length; i++)
            {
                Components[i].Controller = this;
            }
        }
    }
}
