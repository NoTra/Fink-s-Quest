using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStrike : PlayerSystem
{
    private Animator _animator;
    private Striker _striker;

    void Start()
    {
        _animator = Player.GetAnimator();
        _striker =  _animator.gameObject.GetComponent<Striker>();
    }

    void Update()
    {
        
    }

    public void OnStrike(InputAction.CallbackContext context)
    {
        if (context.performed && _animator != null && _striker != null && !_striker._isStriking && Player.GetDrive() != Player.Drive.SOUL)
        {
            Debug.Log("Strike");

            // On lance l'animation
            _animator.SetTrigger("Strike");
            _striker._isStriking = true;

            // _strikeCoroutine = StartCoroutine(PerformStrike());
        }
    }
}
