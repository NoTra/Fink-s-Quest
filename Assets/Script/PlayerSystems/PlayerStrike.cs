using UnityEngine;
using UnityEngine.InputSystem;

using FinksQuest.Core;
using FinksQuest.Behavior;
using System.Collections;

namespace FinksQuest.PlayerSystems
{
    public class PlayerStrike : PlayerSystem
    {
        private Animator _animator;
        private Striker _striker;

        void Start()
        {
            _animator = _player.GetAnimator();
            _striker = _animator.gameObject.GetComponent<Striker>();
        }

        public void OnStrike(InputAction.CallbackContext context)
        {
            if (context.performed && _animator != null && _striker != null && !_striker._isStriking && _player.GetDrive() != Player.Drive.SOUL && _player._canStrike)
            {
                // On lance l'animation
                _animator.SetTrigger("Strike");
                _striker._isStriking = true;

                StartCoroutine(ResetIsStriking());
            }
        }

        private IEnumerator ResetIsStriking()
        {
            float secondsToWait = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(secondsToWait);

            _striker._isStriking = false;
        }
    }
}