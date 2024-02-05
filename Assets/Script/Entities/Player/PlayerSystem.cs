using UnityEngine;

public abstract class PlayerSystem : MonoBehaviour
{
    protected Player Player { get; private set; }

    protected virtual void Awake()
    {
        Player = GetComponent<Player>();
    }
}
