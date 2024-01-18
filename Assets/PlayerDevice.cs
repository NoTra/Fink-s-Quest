using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDevice : MonoBehaviour
{
    public string deviceName;

    // Start is called before the first frame update
    void Start()
    {
        deviceName = GameManager.Instance._playerInput.currentControlScheme;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance._playerInput.currentControlScheme != deviceName)
        {
            deviceName = GameManager.Instance._playerInput.currentControlScheme;

            // On sauvegarde le device actuel
            PlayerPrefs.SetString("PlayerDevice", deviceName);
        }
    }
}
