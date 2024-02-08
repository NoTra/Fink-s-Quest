using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Room _room;
    private Thrower _thrower;

    // Start is called before the first frame update
    void Start()
    {
        _room = GetComponentInParent<Room>();

        if (_room != null)
        {
            _room.AddTower(this);
        } else
        {
            Debug.LogError("Tower " + this.name + " has no parent Room");
        }
            

        _thrower = GetComponentInChildren<Thrower>();
    }

    public void DestroyAllProjectiles()
    {
        foreach (Projectile projectile in _thrower._projectiles)
        {
            Destroy(projectile.gameObject);
        }
    }
}
