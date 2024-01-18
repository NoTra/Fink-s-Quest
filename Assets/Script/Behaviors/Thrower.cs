using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _throwStrength = 5f;
    [SerializeField] private float _timeBetweenThrows = 1f;
    [SerializeField] private float _damage = 1f;

    private float _lastThrowTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Every _timeBetweenThrows seconds, launch ThrowProjectile
        if (Time.time - _lastThrowTime > _timeBetweenThrows)
        {
            _lastThrowTime = Time.time;
            ThrowProjectile();

            Debug.Log("THROW !");
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
    }
}
