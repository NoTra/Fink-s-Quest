using System.Collections.Generic;
using UnityEngine;

using FinksQuest.Entities.Enemies;

namespace FinksQuest.Behavior
{
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
                return true;
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
                return true;
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
                if (CheckButtons() && CheckEnemies())
                {
                    _isOpen = true;
                }
            }
        }
    }
}