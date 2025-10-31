using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerController PlayerController { get; private set; }
        public PlayerStatus PlayerStatus { get; private set; }

        private void Awake()
        {
            PlayerController = FindObjectOfType<PlayerController>();
            PlayerStatus = FindObjectOfType<PlayerStatus>();
        }
    }
}
