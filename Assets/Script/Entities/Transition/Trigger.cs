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
            Debug.Log("Trigger enter on pressure plate : " + gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                if (GameManager.Instance.Player.GetDrive() == Player.Drive.BODY)
                {
                    Debug.Log("Body entered");
                    _isTriggered = true;
                }
                else
                {
                    Debug.Log("Soul entered");
                    GameManager.Instance.Player.GetComponent<PlayerSwitchDrive>().SwitchDrive();
                }
            }
        }
    }
}