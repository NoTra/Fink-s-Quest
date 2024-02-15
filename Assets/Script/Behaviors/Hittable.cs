using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using FinksQuest.Core;
using FinksQuest.Entities.Enemies;
using System.Linq;
using System;

namespace FinksQuest.Behavior
{
    public class Hittable : MonoBehaviour
    {
        [SerializeField] float _maxHP = 3f;
        [SerializeField] public float _currentHP = 3f;

        public bool _isInvincible = false;
        public bool _isHit = false;

        [SerializeField] private Renderer _renderer;
        private Rigidbody _rigidbody;

        public AudioSource _audioSource;
        public AudioClip _hitSound;
        public AudioClip[] _deathSounds;

        public GameObject[] _deathEffects;

        private Color _defaultColor;
        [SerializeField] private Color _targetColor = Color.red;

        [SerializeField] private float _invincibilityDuration = 0.5f;

        // Link to HP Slider
        public Slider _healthBar;

        private void Awake()
        {
            if (_healthBar != null)
            {
                _healthBar.maxValue = _maxHP;
                _healthBar.value = _maxHP;
            }
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();

            _currentHP = _maxHP;
        }

        public IEnumerator FlashRed()
        {
            _isInvincible = true;

            /*
            // Stocker les matériaux d'origine dans une liste
            List<Material> defaultMaterials = _renderer.materials.ToList<Material>();

            // Créer une liste pour stocker les matériaux modifiés
            List<Material> coloredMaterials = new();

            // Cloner les matériaux pour éviter de modifier les originaux
            foreach (Material material in defaultMaterials)
            {
                var coloredMaterial = new Material(material);
                coloredMaterials.Add(coloredMaterial);
                coloredMaterial.color = _targetColor; // Modifier la couleur pour le clignotement
            }
            */

            List<Renderer> allRenderers = new() { _renderer };
            var everyRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in everyRenderers)
            {
                // Ignore the trail renderer
                if (renderer is TrailRenderer)
                {
                    continue;
                }
                
                allRenderers.Add(renderer);
            }

            // allRenderers.AddRange(GetComponentsInChildren<Renderer>());

            float elapsedTime = 0f;
            while (elapsedTime < _invincibilityDuration)
            {
                foreach (Renderer renderer in allRenderers)
                {
                    renderer.enabled = false;
                    // Change color to red
                    // _renderer.materials = coloredMaterials.ToArray();
                }
                // _renderer.enabled = false;
                // Change color to red
                // _renderer.materials = coloredMaterials.ToArray();
                yield return new WaitForSeconds(0.05f);

                // _renderer.enabled = true;
                foreach (Renderer renderer in allRenderers)
                {
                    renderer.enabled = true;
                }
                // Change color to original color
                // _renderer.materials = defaultMaterials.ToArray();
                yield return new WaitForSeconds(0.05f);

                elapsedTime += 0.1f;
            }
            /*
            // Change color to original color
            _renderer.materials = defaultMaterials.ToArray();
            */
            foreach (Renderer renderer in allRenderers)
            {
                renderer.enabled = true;
            }

            _isInvincible = false;
        }

        /**
         * 
         * Called when the creature is hit
         *
         * @param Enemy enemy
         * 
         */
        public void Hit(Vector3 strikerPosition, float strikerStrength, float damage, GameObject striker)
        {
            _currentHP = Mathf.Max(0f, _currentHP - damage);

            if (_healthBar != null)
            {
                _healthBar.value = _currentHP;
            }

            if (_currentHP <= 0f)
            {
                // Play death sound
                if (_deathSounds != null)
                {
                    foreach (AudioClip _deathSound in _deathSounds)
                    {
                        AudioSource.PlayClipAtPoint(_deathSound, transform.position);
                    }
                }

                // Play death effects
                foreach (GameObject effect in _deathEffects)
                {
                    GameObject deathEffect = Instantiate(effect, transform.position, Quaternion.identity);
                    deathEffect.transform.Rotate(-90f, 0f, 0f);

                    Destroy(deathEffect, 2f);
                }

                if (gameObject.CompareTag("Player"))
                {
                    var projectile = striker.GetComponent<Projectile>();

                    if (projectile != null)
                    {
                        projectile._thrower.DestroyAllProjectiles(false);
                    }

                    Debug.Log("Player died");
                    // Restart the level
                    // GameManager.Instance.RestartLevel();
                    StartCoroutine(PlayerDeath());
                }
                else
                {
                    if (GetComponent<Skeleton>() != null)
                    {
                        // Remove the skeleton from the room
                        GameManager.Instance._currentRoom.GetComponent<Room>().RemoveSkeleton(GetComponent<Skeleton>());
                    }

                    Destroy(gameObject);
                }

                return;
            } else{
                if (gameObject.CompareTag("Player"))
                {
                    GameManager.Instance.Player._playerBodyAnimator.SetTrigger("Hit");
                }
            }

            if (_hitSound != null)
            {
                Debug.Log("Play sound of Hittable");
                // Play the impact sound of the hittable
                _audioSource.PlayOneShot(_hitSound);
            }

            StartCoroutine(FlashRed());

            // Make the target move back
            StartCoroutine(MoveBack(strikerPosition, strikerStrength));
        }

        private IEnumerator PlayerDeath()
        {
            GameManager.Instance.Player._playerBodyAnimator.SetTrigger("Death");

            // Wait for 2s
            yield return new WaitForSeconds(2f);
            
            // Début transition
            StartCoroutine(UIManager.Instance.StartRoomTransition());
            
            yield return null;

            // StartCoroutine(UIManager.Instance.EndRoomTransition());

            UIManager.Instance.DisplayGameOverScreen();

            yield return null;
        }

        IEnumerator MoveBack(Vector3 fromPosition, float strength)
        {
            _isHit = true;
            var direction = transform.position - fromPosition;
            direction.Normalize();

            // On ne veut pas que l'ennemi soit déplacé en hauteur
            direction.y = 0f;

            var duration = 0.2f;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                // Appliquer la force graduellement sur plusieurs frames
                _rigidbody.AddForce(strength * Time.deltaTime * direction / duration, ForceMode.VelocityChange);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rigidbody.velocity = Vector3.zero;

            _isHit = false;
        }
    }
}