using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private GameObject _stepEffect;
    [SerializeField] private GameObject _swordTrailEffect;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioManager _audioManager;

    public void Awake()
    {
    }

    IEnumerator Step()
    {
        // Si le joueur est en mode SOUL, on ne fait rien
        if (GameManager.Instance.Player.GetDrive() == Player.Drive.SOUL)
        {
            yield break;
        }
        
        // On stock la position du joueur
        Vector3 playerPosition = transform.position;

        // On attend 0.1s
        yield return new WaitForSeconds(0.1f);

        GameObject stepPoof = Instantiate(_stepEffect, transform.position, Quaternion.identity);

        // On met le stepPoof à la racine de la scène
        stepPoof.transform.SetParent(null);

        _audioSource.PlayOneShot(GameManager.Instance._audioManager._stepSound);
    }

    IEnumerator SwordTrail()
    {
        // On stock la position du joueur
        Vector3 playerPosition = transform.position;

        // On attends 0.1s
        yield return new WaitForSeconds(0.1f);

        // Rotation de base (frappe horizontale)
        Vector3 eulerRotation = new(90f, 90f, 0f);
        Quaternion eulerQuaternion = Quaternion.Euler(eulerRotation);

        // Rotation actuelle du joueur
        Quaternion playerRotation = transform.rotation;

        // On combine les deux rotations
        Quaternion finalRotation = playerRotation * eulerQuaternion;

        GameObject swordTrail = Instantiate(_swordTrailEffect, new Vector3(transform.position.x, 0.33f, transform.position.z), finalRotation);
        swordTrail.transform.SetParent(transform);

        _audioSource.PlayOneShot(GameManager.Instance._audioManager._swordSound);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
