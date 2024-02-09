using UnityEngine;

using FinksQuest.Core;

namespace FinksQuest.Interactible
{
    public class Crate : MonoBehaviour
    {
        public Vector3 _startingPosition;
        private Room _room;

        // Start is called before the first frame update
        void Start()
        {
            _startingPosition = transform.position;

            _room = GetComponentInParent<Room>();

            if (_room != null)
            {
                _room.AddCrate(this);
            }
        }
    }
}
