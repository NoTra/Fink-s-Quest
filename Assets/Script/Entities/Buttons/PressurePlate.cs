using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Activable
{
    MeshRenderer _meshRenderer;

    Material _emitterMaterial;
    [SerializeField] float _emitterIntensityOff = 0f;
    [SerializeField] float _emitterIntensityOn = 10f;
    [SerializeField] float _transitionSpeed = 0.5f;
    [SerializeField] Color _startEmissionColor;

    private AudioSource _audioSource;

    private Coroutine _dimmUpCoroutine;
    private Coroutine _dimmDownCoroutine;


    private void Awake()
    {
        Debug.Log("Button awaking...");
        _meshRenderer = GetComponent<MeshRenderer>();
        _emitterMaterial = _meshRenderer.materials[1];

        Material[] materials = _meshRenderer.materials;
        materials[1].SetColor("_EmissionColor", _startEmissionColor * _emitterIntensityOff);

        _audioSource = GetComponent<AudioSource>();
    }

    new public void Activate()
    {
        if (_dimmDownCoroutine != null) {
            StopCoroutine(_dimmDownCoroutine);
            _dimmDownCoroutine = null;
        }
            

        Debug.Log("Button activated");
        base.Activate();

        if (_dimmUpCoroutine == null)
        {
            Debug.Log("DimmUpCoroutine");
            _dimmUpCoroutine = StartCoroutine(DimmUp());
        } else
        {
            if (_dimmDownCoroutine != null)
            {
                Debug.Log("Stop DimmDownCoroutine");
                StopCoroutine(_dimmDownCoroutine);
                _dimmDownCoroutine = null;
            }

            Debug.Log("Force EmissionColor");
            _emitterMaterial.SetColor("_EmissionColor", _startEmissionColor * _emitterIntensityOn);
        }
    }

    IEnumerator DimmUp()
    {
        Debug.Log("DimmUp");
        float elapsedTime = 0f;

        if (GameManager.Instance._playerController._player._drive != Player.Drive.SOUL) {
            // On lance le son de pression de bouton
            if (GameManager.Instance._audioManager._pressurePlatePressedSound != null)
            {
                _audioSource.PlayOneShot(GameManager.Instance._audioManager._pressurePlatePressedSound);

                if (GameManager.Instance._audioManager._pressurePlatePressedSound != null)
                {
                    _audioSource.PlayOneShot(GameManager.Instance._audioManager._switchButtonSound);
                }
            }
        }

        while (elapsedTime < _transitionSpeed)
        {
            float intensity = Mathf.Lerp(_emitterIntensityOff, _emitterIntensityOn, (elapsedTime / _transitionSpeed));
            _emitterMaterial.SetColor("_EmissionColor", _startEmissionColor * intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _dimmUpCoroutine = null;
    }

    IEnumerator DimmDown()
    {
        Debug.Log("DimmDown");
        float elapsedTime = 0f;

        while (elapsedTime < _transitionSpeed)
        {
            float intensity = Mathf.Lerp(_emitterIntensityOn, _emitterIntensityOff, (elapsedTime / _transitionSpeed));
            _emitterMaterial.SetColor("_EmissionColor", _startEmissionColor * intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _dimmDownCoroutine = null;
    }

    new public void Deactivate()
    {
        base.Deactivate();

        if (_dimmUpCoroutine != null)
        {
            StopCoroutine(_dimmUpCoroutine);
            _dimmUpCoroutine = null;
        }

        Debug.Log("Deactivate");

        _dimmDownCoroutine = StartCoroutine(DimmDown());
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
        Activate();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isActive)
        {
            Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Deactivate();
    }
}
