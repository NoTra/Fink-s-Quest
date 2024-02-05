using System.Collections;
using UnityEngine;

public class PressurePlate : Activable
{
    MeshRenderer _meshRenderer;

    Material _emitterMaterial;
    [SerializeField] float _emitterIntensityOff = -10f;
    [SerializeField] float _emitterIntensityOn = 10f;
    [SerializeField] float _transitionSpeed = 0.5f;
    Color _startEmissionColor;

    private AudioSource _audioSource;

    private Coroutine _dimmUpCoroutine;
    private Coroutine _dimmDownCoroutine;


    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _emitterMaterial = _meshRenderer.materials[1];

        Material[] materials = _meshRenderer.materials;
        _startEmissionColor = materials[1].GetColor("_EmissionColor");

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

        while (elapsedTime < _transitionSpeed)
        {
            float intensity = Mathf.Lerp(_emitterIntensityOff, _emitterIntensityOn, (elapsedTime / _transitionSpeed));
            _emitterMaterial.SetColor("_EmissionColor", _startEmissionColor * intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter on pressure plate");
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
        Debug.Log("Trigger exit");
        // Check if something is still intersect with BoxCollider
        if (Physics.CheckBox(transform.position, transform.localScale / 2f, transform.rotation, LayerMask.GetMask("Player", "Soul", "Grabbable")))
        {
            Debug.Log("Something is still on the button");
            return;
        }

        Deactivate();
    }
}
