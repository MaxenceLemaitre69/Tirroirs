using UnityEngine;

namespace Plat.Gameplay
{
    public class PlayerCamera : MonoBehaviour, IPlayerComponent
    {
        public PlayerController Controller { get; set; }
    }
}