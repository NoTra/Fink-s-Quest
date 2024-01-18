using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private SphereCollider _collider;
    [SerializeField] private GameObject _impactParticle;
    public float _throwStrength;
    public float _damage = 1f;
    [SerializeField] private string[] _invulnerableTag;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit {other.gameObject.name}");
        // Il touche un objet Hittable ? (peut faire du dégât aux ennemis)
        Hittable hittable = other.gameObject.GetComponent<Hittable>();
        // If hittable and 
        if (hittable != null)
        {
            // if not invulnerable to this projectile
            if (!_invulnerableTag.Contains(other.tag))
            {
                hittable.Hit(transform.position, _throwStrength, _damage);
            }
            
            Destroy(gameObject);
        }

        // Si le projectile touche un layer "Ground" ou "Wall", on le détruit
        if (
            other.gameObject.layer == LayerMask.NameToLayer("Ground") || 
            other.gameObject.layer == LayerMask.NameToLayer("Wall") || 
            other.gameObject.layer == LayerMask.NameToLayer("Grabbable")
        ) {
            Debug.Log("Projectile hit a wall or the ground");

            // On instancie une particule d'impact
            /*GameObject impactParticle = Instantiate(_impactParticle, transform.position, Quaternion.identity);
            impactParticle.transform.position = transform.position;

            // On déplace la particule d'impact afin qu'elle soit posée sur le mur
            impactParticle.transform.position = new Vector3(impactParticle.transform.position.x, impactParticle.transform.position.y, impactParticle.transform.position.z);

            var wallNormal = other.transform.forward;
            // On aligne la rotation de la particule d'impact avec la normale du mur
            impactParticle.transform.rotation = Quaternion.LookRotation(wallNormal);
            */
            
            Destroy(gameObject);
        }
    }
}
