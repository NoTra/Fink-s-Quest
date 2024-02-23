using UnityEngine;

using FinksQuest.Behavior;

namespace FinksQuest
{
    public class Heart : MonoBehaviour
    {
        [SerializeField] private int _healthValue = 1;
        [SerializeField] private AudioClip _healthSound;

        [SerializeField] private GameObject _heartAnimation;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Destroy(gameObject);

                var hittablePlayer = other.gameObject.GetComponent<Hittable>();
                // Add 1 heart to the player's health
                hittablePlayer._currentHP = Mathf.Clamp(hittablePlayer._currentHP + _healthValue, 0, hittablePlayer._maxHP);

                hittablePlayer._healthBar.value = hittablePlayer._currentHP;

                // instantiate the heart animation on the player 1.734
                Instantiate(_heartAnimation, new Vector3(other.transform.position.x, 2f, other.transform.position.z), Quaternion.identity, other.transform);

                if (_healthSound != null)
                {
                    hittablePlayer._audioSource.PlayOneShot(_healthSound);
                }
            }
        }
    }
}
