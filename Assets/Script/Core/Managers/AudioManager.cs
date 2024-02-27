using UnityEngine;

namespace FinksQuest.Core
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip _stepSound;
        public AudioClip _swordSound;
        public AudioClip _secretFoundSound;
        public AudioClip _switchDriveSound;
        public AudioClip _switchButtonSound;
        public AudioClip _pressurePlatePressedSound;
        public AudioClip _openDoorSound;
        public AudioClip _closeDoorSound;
        public AudioClip _swordHitSound;
        public AudioClip _hiddenOpenDoorSound;
        public AudioClip _typingSound;
        public AudioClip _laserShotSound;
        public AudioClip _laserImpactSound;
        public AudioClip _roomClearedSound;
        public AudioClip _lightOnSound;

        public AudioSource _audioSource;


        // Singleton
        public static AudioManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
}