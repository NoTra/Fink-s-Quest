using FinksQuest.Core;
using System.Collections;
using UnityEngine;

namespace FinksQuest
{
    public class ChangeMusic : MonoBehaviour
    {
        [SerializeField] private AudioClip _finalMusic;
        private AudioSource _audioSource;

        private float _fadeDuration = 2f;
        private float _currentTime = 0f;

        // Start is called before the first frame update
        void Start()
        {
            _audioSource = AudioManager.Instance.GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CrossFadeToFinalMusic();
            }
        }

        private void CrossFadeToFinalMusic()
        {
            // Si aucune musique finale n'est spécifiée, ne rien faire
            if (_finalMusic == null) return;

            StartCoroutine(CrossFadeCoroutine());
        }

        private IEnumerator CrossFadeCoroutine()
        {
            float startVolume = 1f;
            Debug.Log("CrossFadeCoroutine startVolume : " + startVolume);
            float targetVolume = 0f;

            // Fondu enchaîné vers le silence
            while (_currentTime < _fadeDuration)
            {
                _currentTime += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(1f, 0f, _currentTime / _fadeDuration);
                yield return null;
            }

            _audioSource.volume = 0f;

            Debug.Log("CrossFadeCoroutine volume end : " + _audioSource.volume);

            // Changer la musique
            _audioSource.clip = _finalMusic;
            _audioSource.Play();

            // Réinitialiser le temps
            _currentTime = 0f;

            // Fondu enchaîné depuis le silence vers le nouveau volume
            targetVolume = startVolume;
            while (_currentTime < 4f)
            {
                _currentTime += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(0f, 1f, _currentTime / 4f);
                yield return null;
            }

            _audioSource.volume = 1f;
        }
    }
}
