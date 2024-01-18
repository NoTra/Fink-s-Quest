using UnityEngine;
using System.Collections;

public class Striker : MonoBehaviour
{
    public bool _isStriking = false;
    [SerializeField] private float _strikeDuration = 0.2f;
    [SerializeField] private float _strikeDelay = 0.2f;
    public float _strikeRange = 0.5f;
    [SerializeField] private float _timeBetweenStrikes = 1f;
    [SerializeField] private AnimationCurve _strikeCurve;
    public float _strikeStrength = 10f;
    [SerializeField] private float _strikeStartAngle = 90f;
    [SerializeField] private float _strikeEndAngle = -90f;

    [SerializeField] private float _damage;
    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _impactEffect;

    [SerializeField] private AudioSource _audioSource;

    // Layer to exclude from the raycast
    public LayerMask _layerToExclude;

    private bool _hitSomething = false;

    public string _targetTag;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }
    
    private void Start()
    {
        
    }

    public void Strike()
    {
        if (_isStriking)
        {
            return;
        }

        StartCoroutine(PerformStrike());
    }

    private IEnumerator PerformStrike()
    {
        float elapsedTime = 0f;

        if (_animator != null)
        {
            _animator.SetTrigger("Strike");
        }

        _isStriking = true;
        
        while(elapsedTime < _strikeDelay)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < _strikeDuration && _hitSomething == false)
        {
            var angle = Mathf.Lerp(_strikeStartAngle, _strikeEndAngle, _strikeCurve.Evaluate(elapsedTime / _strikeDuration));

            Quaternion eulerAngle = Quaternion.Euler(0f, angle, 0f);

            // On draw un ray partant de la position du joueur de longueur 1, commençant à 45° sur la droite et lui faire faire une rotation à 180° vers la gauche
            Debug.DrawRay(transform.position, eulerAngle * transform.forward * _strikeRange, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, eulerAngle * transform.forward, out hit, _strikeRange, ~_layerToExclude))
            {
                Debug.Log("Hit something : " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.tag != null && hit.collider.gameObject.CompareTag(_targetTag))
                {
                    Hittable hitted = hit.collider.gameObject.GetComponent<Hittable>();
                    if (hitted != null && !hitted._isInvincible)
                    {
                        hitted.Hit(transform.position, _strikeStrength, _damage);
                    }
                }


                if (hit.collider.GetComponent<Hittable>() != null && hit.collider.GetComponent<Hittable>()._hitSound != null)
                {
                    _audioSource.PlayOneShot(hit.collider.GetComponent<Hittable>()._hitSound);
                } else if (_audioSource != null)
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