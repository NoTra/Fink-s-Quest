using System.Collections;
using UnityEngine;

using FinksQuest.Core;

namespace FinksQuest.Interactible
{
    public class PushButton : Behavior.Activable
    {
        MeshRenderer _meshRenderer;
        Material _emitterMaterial;
        [SerializeField] float _emitterIntensityOff = -10f;
        [SerializeField] float _emitterIntensityOn = 10f;
        [SerializeField] float _transitionSpeed = 0.5f;
        Color _startEmissionColor;

        private Coroutine _dimmUpCoroutine;
        private Coroutine _dimmDownCoroutine;

        private AudioSource _audioSource;

        // Start is called before the first frame update
        void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _emitterMaterial = _meshRenderer.materials[1];
            _audioSource = GetComponent<AudioSource>();

            Material[] materials = _meshRenderer.materials;
            _startEmissionColor = materials[1].GetColor("_EmissionColor");

            materials[1].SetColor("_EmissionColor", _startEmissionColor * _emitterIntensityOff);

            _isActive = false;
        }

        new public void Activate()
        {
            if (_dimmDownCoroutine != null)
                StopCoroutine(_dimmDownCoroutine);

            // Call parent class
            base.Activate();

            if (GameManager.Instance._audioManager._switchButtonSound != null)
            {
                _audioSource.PlayOneShot(GameManager.Instance._audioManager._switchButtonSound);
            }

            if (_dimmUpCoroutine == null)
            {
                _dimmUpCoroutine = StartCoroutine(DimmUp());
            }
            else
            {
                if (_dimmDownCoroutine != null)
                {
                    StopCoroutine(_dimmDownCoroutine);
                    _dimmDownCoroutine = null;
                }

                _emitterMaterial.SetColor("_EmissionColor", _startEmissionColor * _emitterIntensityOn);
            }
        }

        new public void Toggle()
        {
            if (!_isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        IEnumerator DimmUp()
        {
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
            }

            _dimmDownCoroutine = StartCoroutine(DimmDown());
        }
    }
}