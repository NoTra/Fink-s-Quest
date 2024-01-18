using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Constante "drive" : BODY or SOUL
    public enum Drive
    {
        BODY,
        SOUL
    }

    public Drive _drive = Drive.BODY;

    // Player's body
    [SerializeField] private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Draw a raycast in front of the player to see his orientation
        Debug.DrawRay(_playerController._playerRigidbody.transform.position, _playerController._playerRigidbody.transform.forward * 0.2f, Color.red);
    }
}
