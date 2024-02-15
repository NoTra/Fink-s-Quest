﻿using UnityEngine;
using System.Collections;

using FinksQuest.Core;

namespace FinksQuest.Behavior
{
    public class Striker : MonoBehaviour
    {
        public bool _isStriking = false;
        [SerializeField] private float _strikeDuration = 0.2f;
        public float _strikeRange = 0.5f;
        [SerializeField] private float _timeBetweenStrikes = 1f;
        [SerializeField] private AnimationCurve _strikeCurve;
        public float _strikeStrength = 10f;
        [SerializeField] private float _strikeStartAngle = 90f;
        [SerializeField] private float _strikeEndAngle = -90f;

        [SerializeField] private float _damage;

        [SerializeField] private GameObject _impactEffect;
        [SerializeField] private GameObject _destroyEffect;

        [SerializeField] private AudioSource _audioSource;

        // Layer to exclude from the raycast
        public LayerMask _layerToExclude;

        private bool _hitSomething = false;

        public string _targetTag;

        private void Awake()
        {
        }

        private void Start()
        {

        }

        public void Strike()
        {
            if (_isStriking && GameManager.Instance.Player.GetDrive() != Player.Drive.SOUL)
            {
                return;
            }

            StartCoroutine(PerformStrike());
        }

        private IEnumerator PerformStrike()
        {
            Debug.Log("perform strike");
            float elapsedTime = 0f;
            _isStriking = true;

            while (elapsedTime < _strikeDuration && _hitSomething == false)
            {
                var angle = Mathf.Lerp(_strikeStartAngle, _strikeEndAngle, _strikeCurve.Evaluate(elapsedTime / _strikeDuration));

                Quaternion eulerAngle = Quaternion.Euler(0f, angle, 0f);

                Vector3 origin = new Vector3(transform.position.x, 0.3f, transform.position.z);

                // On draw un ray partant de la position du joueur de longueur 1, commençant à 45° sur la droite et lui faire faire une rotation à 180° vers la gauche
                Debug.DrawRay(origin, eulerAngle * transform.forward * _strikeRange, Color.red);

                RaycastHit hit;
                if (Physics.Raycast(origin, eulerAngle * transform.forward, out hit, _strikeRange, ~_layerToExclude))
                {
                    Hittable hitted = hit.collider.GetComponent<Hittable>();

                    if (hitted != null && !hitted._isInvincible)
                    {
                        hitted.Hit(transform.position, _strikeStrength, _damage, gameObject);
                    }

                    if (hitted == null || hitted._hitSound == null)
                    {
                        // Play the default impact sound
                        _audioSource.PlayOneShot(GameManager.Instance._audioManager._swordHitSound);
                    }

                    if (_impactEffect != null)
                    {
                        GameObject currentImpact = Instantiate(_impactEffect, hit.point, Quaternion.identity);

                        // Wait 1s before destroying the impact effect
                        Destroy(currentImpact, 1f);
                    }

                    _hitSomething = true;
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            _hitSomething = false;

            yield return new WaitForSeconds(_timeBetweenStrikes);

            _isStriking = false;
        }
    }
}