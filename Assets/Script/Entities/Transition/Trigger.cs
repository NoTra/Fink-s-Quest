using UnityEngine;
using FinksQuest.Core;
using FinksQuest.PlayerSystems;

namespace FinksQuest.Entities.Transition
{
    public class Trigger : MonoBehaviour
    {
        public bool _isTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Soul"))
            {
                if (GameManager.Instance.Player.GetDrive() == Player.Drive.BODY)
                {
                    _isTriggered = true;
                }
                else
                {
                    GameManager.Instance.Player.GetComponent<PlayerSwitchDrive>().SwitchDrive();
                }
            }
        }
    }
}