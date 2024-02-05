using UnityEngine;

public class PlayerDevice : MonoBehaviour
{
    public string deviceName;

    // Start is called before the first frame update
    void Start()
    {
        deviceName = GameManager.Instance.Player._playerInput.currentControlScheme;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Player._playerInput.currentControlScheme != deviceName)
        {
            deviceName = GameManager.Instance.Player._playerInput.currentControlScheme;

            // On sauvegarde le device actuel
            PlayerPrefs.SetString("PlayerDevice", deviceName);
        }
    }
}
