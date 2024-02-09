using UnityEngine;
using FinksQuest.Core;

namespace FinksQuest.PlayerSystems
{
    public abstract class PlayerSystem : MonoBehaviour
    {
        protected Player _player { get; private set; }

        protected virtual void Awake()
        {
            _player = GetComponent<Player>();
        }
    }
}
