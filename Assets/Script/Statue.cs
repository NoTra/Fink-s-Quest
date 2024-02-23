using FinksQuest.Behavior;
using FinksQuest.Entities.Door;
using UnityEngine;

namespace FinksQuest
{
    public class Statue : Activable
    {
        [SerializeField] private HiddenDoor _hiddenDoor;
        public Light _light;

        // Update is called once per frame
        void Update()
        {
            if (_hiddenDoor._isOpen)
            {
                _light.gameObject.SetActive(true);
                _isActive = true;
            }
            else
            {
                _light.gameObject.SetActive(false);
                _isActive = false;
            }
        }
    }
}
