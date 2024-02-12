using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FinksQuest.Core;
using FinksQuest.Entities.Enemies;

namespace FinksQuest.Behavior
{
    public class Thrower : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private GameObject _projectile;
        [SerializeField] private float _throwStrength = 5f;
        [SerializeField] private float _timeBetweenThrows = 1f;
        [SerializeField] private float _damage = 1f;
        [SerializeField] private GameObject Ball;
        private AudioSource _audioSource;

        public List<Projectile> _projectiles = new List<Entities.Enemies.Projectile>();

        public bool isActivated = true;
        private GameObject _currentRoom;

        private float _lastThrowTime;

        // Start is called before the first frame update
        void Start()
        {
            _currentRoom = GetComponentInParent<Room>().gameObject;
            _audioSource = GetComponent<AudioSource>();

            _lastThrowTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            // Every _timeBetweenThrows seconds, launch ThrowProjectile
            if (_currentRoom == GameManager.Instance._currentRoom && isActivated && (Time.time - _lastThrowTime) > _timeBetweenThrows)
            {
                _lastThrowTime = Time.time;
                ThrowProjectile();
            }
        }

        // Throw a projectile at the player position
        private void ThrowProjectile()
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = _target.transform.position;

            // On définit une direction pour le projectile
            Vector3 direction = targetPosition - startPosition;

            // Instantiate projectile
            GameObject projectile = Instantiate(_projectile, startPosition, Quaternion.identity);
            // Set projectile's target tag
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript._throwStrength = _throwStrength;
            projectileScript._damage = _damage;

            projectile.GetComponent<Rigidbody>().AddForce(direction.normalized * _throwStrength, ForceMode.Impulse);

            projectileScript._thrower = this;

            _projectiles.Add(projectileScript);

            _audioSource.PlayOneShot(GameManager.Instance._audioManager._laserShotSound);
            Destroy(projectile, 10f);
        }

        public void DestroyAllProjectiles(bool showEffects = true)
        {
            var _projectilesCopy = new List<Entities.Enemies.Projectile>(_projectiles);
            foreach (Entities.Enemies.Projectile projectile in _projectilesCopy)
            {
                projectile.DestroySelf(showEffects);
            }

            if (showEffects)
            {
                _audioSource.PlayOneShot(GameManager.Instance._audioManager._laserImpactSound);
                _projectiles.Clear();

                // On éteint la tour
                Color startColor = Ball.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
                StartCoroutine(LerpMaterialColor(startColor, Color.black, 1f));
            }

        }

        IEnumerator LerpMaterialColor(Color startColor, Color endColor, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                Ball.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Ball.GetComponent<Animation.FloatAnimation>().enabled = false;
        }
    }
}