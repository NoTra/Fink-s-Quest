using FinksQuest.Behavior;
using FinksQuest.Entities.Door;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FinksQuest
{
    public class Statue : Activable
    {
        [SerializeField] private HiddenDoor _hiddenDoor;
        [SerializeField] private Light _light;

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
