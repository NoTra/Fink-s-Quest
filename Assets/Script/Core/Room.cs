using System.Collections.Generic;
using UnityEngine;

using FinksQuest.UI;
using FinksQuest.Entities.Enemies;
using FinksQuest.Interactible;


namespace FinksQuest.Core
{
    public class Room : MonoBehaviour
    {
        public Vector3 _cameraPosition;
        public float _maxZoom;
        public bool _isResolved = false;
        private List<Crate> _crates = new List<Crate>();
        private List<Skeleton> _skeletons = new List<Skeleton>();
        private List<Tower> _towers = new List<Tower>();

        private DialogBox _dialogBox;

        private void Awake()
        {
            _dialogBox = GetComponent<DialogBox>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // On est la room courante ? 
            if (GameManager.Instance._currentRoom == this.gameObject && _dialogBox != null)
            {
                _dialogBox.enabled = true;
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("Player");

            if (GameManager.Instance._currentRoom == this.gameObject && _dialogBox != null && _dialogBox._message != "")
            {
                _dialogBox.enabled = true;
            }
        }

        public void AddTower(Tower tower)
        {
            _towers.Add(tower);
        }

        public void AddCrate(Crate crate)
        {
            _crates.Add(crate);
        }

        public void AddSkeleton(Skeleton skeleton)
        {
            _skeletons.Add(skeleton);
        }

        public void RemoveSkeleton(Skeleton skeleton)
        {
            _skeletons.Remove(skeleton);
        }

        public void OnLeaveRoom()
        {
            if (!_isResolved)
            {
                foreach (Crate crate in _crates)
                {
                    crate.transform.position = crate._startingPosition;
                }

                foreach (Skeleton skeleton in _skeletons)
                {
                    skeleton.transform.position = skeleton._startingPosition;
                }
            }

            foreach (Tower tower in _towers)
            {
                tower.DestroyAllProjectiles();
            }
        }
    }
}