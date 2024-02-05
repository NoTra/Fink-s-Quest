using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Hittable : MonoBehaviour
{
    [SerializeField] float _maxHP = 3f;
    [SerializeField] float _currentHP = 3f;

    public bool _isInvincible = false;
    public bool _isHit = false;

    [SerializeField] private Renderer _renderer;
    private Rigidbody _rigidbody;

    public AudioClip _hitSound;
    public AudioClip[] _deathSounds;

    public GameObject[] _deathEffects;

    private Color _defaultColor;
    [SerializeField] private Color _targetColor = Color.red;

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

        _currentHP = _maxHP;
    }

    public IEnumerator FlashRed()
    {
        Debug.Log("Flash red !!!!!!!");

        // Stocker les matériaux d'origine dans une liste
        List<Material> defaultMaterials = new List<Material>(_renderer.materials);

        // Créer une liste pour stocker les matériaux modifiés
        List<Material> coloredMaterials = new List<Material>();

        // Cloner les matériaux pour éviter de modifier les originaux
        foreach (Material material in defaultMaterials)
        {
            Material coloredMaterial = new Material(material);
            coloredMaterials.Add(coloredMaterial);
            coloredMaterial.color = _targetColor; // Modifier la couleur pour le clignotement
        }

        // Make material blink 4 times ((0.05 * 2) * 4 = 0.4s)
        for (int i = 0; i < 4; i++)
        {
            // Change color to red
            _renderer.materials = coloredMaterials.ToArray();
            yield return new WaitForSeconds(0.05f);

            // Change color to original color
            _renderer.materials = defaultMaterials.ToArray();
            yield return new WaitForSeconds(0.05f);
        }

        // Change color to original color
        _renderer.materials = defaultMaterials.ToArray();
    }

    /**
     * 
     * Called when the enemy is hit
     *
     * @param Enemy enemy
     * 
     */
    public void Hit(Vector3 strikerPosition, float strikerStrength, float damage)
    {
        Debug.Log("Hit !!!!!!!");
        _isInvincible = true;

        _currentHP = Mathf.Max(0f, _currentHP - damage);

        if (_healthBar != null)
        {
            _healthBar.value = _currentHP;
        }
        
        // Make the player flash red
        StartCoroutine(FlashRed());

        // Make the player move back
        StartCoroutine(MoveBack(strikerPosition, strikerStrength));
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
        _isInvincible = false;
    }

    private void Update()
    {
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
                Debug.Log("Player died");
                // Restart the level
                GameManager.Instance.RestartLevel();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}