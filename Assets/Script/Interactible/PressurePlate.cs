using System.Collections;
using UnityEngine;

namespace FinksQuest.Interactible
{
    public class PressurePlate : Behavior.Activable
    {
        MeshRenderer _meshRenderer;

        Material _emitterMaterial;
        [SerializeField] float _emitterIntensityOff = 0f;
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
            if (_dimmDownCoroutine != null)
            {
                StopCoroutine(_dimmDownCoroutine);
                _dimmDownCoroutine = null;
            }

            base.Activate();

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

            _emitterMaterial.SetColor("_EmissionColor", _startEmissionColor * _emitterIntensityOn);
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

            _emitterMaterial.SetColor("_EmissionColor", _startEmissionColor * _emitterIntensityOff);
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
            Debug.Log("OnTriggerEnter");
            // Debug.Log("POUET : " + other.gameObject.layer.ToString());
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("Ghost") ||
                other.gameObject.layer == LayerMask.NameToLayer("Grabbable")
            )
            {
                Activate();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Debug.Log("OnTriggerStay");
            if (!_isActive &&
                (
                other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("Ghost") ||
                other.gameObject.layer == LayerMask.NameToLayer("Grabbable")
                )
            )
            {
                Activate();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("OnTriggerExit");
            // Wait half a second before checking if something is still on the pressure plate
            // StartCoroutine(IsStillOnPressurePlate());

            var boxCollider = GetComponent<BoxCollider>();
            var boxSize = boxCollider.bounds.size * 0.5f;
            // Debug.DrawRay(transform.position, transform.forward * boxCollider.bounds.size.x * 0.5f, Color.red, 1f);

            // Vector3 boxSize = transform.localScale * 0.5f;

            LayerMask layerMask = LayerMask.GetMask("Player", "Ghost", "Grabbable");

            // Check if something is still intersect with BoxCollider
            if (Physics.OverlapBox(transform.position, boxSize, transform.rotation, layerMask).Length == 0)
            {
                
                Deactivate();

                return;
            }

            Debug.Log("Something is still on the pressure plate");
            Debug.Log(Physics.OverlapBox(transform.position, boxSize, transform.rotation, layerMask)[0].name);

        }

        private IEnumerator IsStillOnPressurePlate()
        {
            // Wait half a second
            // yield return new WaitForSeconds(0.4f);

            var boxCollider = GetComponent<BoxCollider>();
            var boxSize = boxCollider.bounds.size;

            // Vector3 boxSize = transform.localScale * 0.5f;

            LayerMask layerMask = LayerMask.GetMask("Player", "Ghost", "Grabbable");

            // Check if something is still intersect with BoxCollider
            if (Physics.OverlapBox(transform.position, boxSize, transform.rotation, layerMask).Length > 0)
            {
                Debug.Log("Something is still on the pressure plate");
                Debug.Log(Physics.OverlapBox(transform.position, boxSize, transform.rotation, layerMask)[0].name);
                // Kill the coroutine
                yield break;
            }

            Deactivate();
        }
    }
}
