using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

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
