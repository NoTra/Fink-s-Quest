using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Openable
{
    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private Color _color;
    [SerializeField] private float intensity;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        Material[] materials = _meshRenderer.materials;
        materials[1].SetColor("_EmissionColor", _color * intensity);

        _meshRenderer.materials = materials;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (_isOpen == _previousOpenState)
        {
            return;
        }

        if (_isOpen)
        {
            _animator.SetBool("isOpen", true);

            // Play sound doorOpen
            _audioSource.PlayOneShot(GameManager.Instance._audioManager._openDoorSound);
            _previousOpenState = _isOpen;
        }
        else
        {
            _animator.SetBool("isOpen", false);
            // Play sound doorClose
            _audioSource.PlayOneShot(GameManager.Instance._audioManager._closeDoorSound);
            _previousOpenState = _isOpen;
        }
    }


}
