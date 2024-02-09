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
            Debug.Log("Trigger enter on pressure plate");
            if (other.gameObject.CompareTag("Player") && GameManager.Instance.Player.GetDrive() == Player.Drive.BODY)
            {
                Debug.Log("Body entered");
                _isTriggered = true;
            }
            else
            {
                GameManager.Instance.Player.GetComponent<PlayerSwitchDrive>().SwitchDrive();
            }
        }
    }
}