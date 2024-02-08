using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Thrower Thrower;
    private SphereCollider _collider;    
    public float _throwStrength;
    public float _damage = 1f;
    [SerializeField] private string[] _invulnerableTag;
    [SerializeField] private GameObject _impactParticle;
    private AudioSource _audioSource;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit {other.gameObject.name}");
        // Il touche un objet Hittable ? (peut faire du d�g�t aux ennemis)
        Hittable hittable = other.gameObject.GetComponent<Hittable>();
        // If hittable and 
        if (hittable != null)
        {
            // if not invulnerable to this projectile
            if (!_invulnerableTag.Contains(other.tag))
            {
                hittable.Hit(transform.position, _throwStrength, _damage);
            }

            Debug.Log("Play impact sound");
            AudioSource.PlayClipAtPoint(GameManager.Instance._audioManager._laserImpactSound, transform.position);

            DestroySelf();
        }

        // Si le projectile touche un layer "Ground" ou "Wall", on le d�truit
        if (
            other.gameObject.layer == LayerMask.NameToLayer("Ground") || 
            other.gameObject.layer == LayerMask.NameToLayer("Wall") || 
            other.gameObject.layer == LayerMask.NameToLayer("Grabbable")
        ) {
            Debug.Log("Play impact sound");
            AudioSource.PlayClipAtPoint(GameManager.Instance._audioManager._laserImpactSound, transform.position);

            DestroySelf();
        }
    }

    public void DestroySelf(bool showEffects = true)
    {
        if (showEffects)
        {
            // On instancie une particule d'impact
            GameObject impactParticle = Instantiate(_impactParticle, transform.position, Quaternion.identity);

            // On d�truit la particule d'impact apr�s 1 seconde
            Destroy(impactParticle, 1f);
        }

        Thrower._projectiles.Remove(this);
        Destroy(gameObject);
    }
}
