using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public Vector3 _startingPosition;
    private Room _room;

    // Start is called before the first frame update
    void Start()
    {
        _startingPosition = transform.position;
        _room = GetComponentInParent<Room>();
        _room.AddCrate(this);
    }
}
