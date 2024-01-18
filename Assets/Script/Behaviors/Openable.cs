using System.Collections.Generic;
using UnityEngine;

public class Openable : MonoBehaviour
{
    public bool _isOpen = false;
    public bool _previousOpenState = false;

    // List of ActivableButton needed to open
    [SerializeField] List<Activable> _activableButtons = new List<Activable>();

    // List of Enemies to kill
    [SerializeField] List<Skeleton> _enemiesToKill = new List<Skeleton>();

    // Start is called before the first frame update
    void Start()
    {
        _previousOpenState = _isOpen;
    }

    private bool CheckEnemies()
    {
        if (_enemiesToKill.Count == 0)
        {
            return false;
        }

        // Check if all gameObject Enemies are Destroyed
        foreach (Skeleton enemy in _enemiesToKill)
        {
            if (enemy != null)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckButtons()
    {
        if (_activableButtons.Count == 0)
        {
            return false;
        }

        foreach (Activable activable in _activableButtons)
        {
            if (!activable._isActive)
            {
                return false;
            }
        }

        return true;
    }

    public void Update()
    {
        if (!_isOpen)
        {
            if (_activableButtons.Count > 0 && CheckButtons())
            {
                Debug.Log("Openable is open (buttons)");
                _isOpen = true;

            }
            if (_enemiesToKill.Count > 0 && CheckEnemies())
            {
                Debug.Log("Openable is open (enemies)");
                _isOpen = true;
            }
        }
    }
}