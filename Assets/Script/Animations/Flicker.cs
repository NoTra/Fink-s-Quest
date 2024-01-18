using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Flicker : MonoBehaviour
{
    private Light _light;
    private float _timeBetweenFlickers = 0.1f;
    private float _timeBetweenMovements = 0.1f;
    private bool _isFlickering = false;
    [SerializeField] float _intensityRange = 0.5f;
    private Vector3 _startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        _startingPosition = _light.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float minIntensity = _light.intensity - _intensityRange;
        float maxIntensity = _light.intensity + _intensityRange;
        float lightIntensity = Random.Range(minIntensity, maxIntensity);
        Vector3 lightMovement = new (Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
        Vector3 targetPosition = _startingPosition + lightMovement;

        if (!_isFlickering)
        {
            _isFlickering = true;
            StartCoroutine(FlickerLight(lightIntensity, targetPosition));
        }
    }

    IEnumerator FlickerLight(float targetLightIntensity, Vector3 targetPosition)
    {
        _timeBetweenFlickers = Random.Range(0.05f, 0.1f);
        _timeBetweenMovements = Random.Range(0.1f, 0.2f);

        var startingLightIntensity = _light.intensity;

        var elapsedTime = 0f;
        while (elapsedTime < _timeBetweenFlickers || elapsedTime < _timeBetweenMovements)
        {
            if (elapsedTime < _timeBetweenFlickers)
            {
                _light.intensity = Mathf.Lerp(startingLightIntensity, targetLightIntensity, elapsedTime / _timeBetweenFlickers);
            }

            if (elapsedTime < _timeBetweenMovements)
            {
                _light.transform.position = Vector3.Lerp(_startingPosition, targetPosition, elapsedTime / _timeBetweenFlickers);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _isFlickering = false;
    }
}
