using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Make the object float between -0.2 and -0.2696
        float sinY = Mathf.Sin(Time.time);
        float scaledY = Mathf.LerpUnclamped(-0.2696f, -0.2f, sinY);
        float clampedY = Mathf.Clamp(scaledY, -0.2696f, -0.2f);

        transform.localPosition = new Vector3(0f, clampedY, 0f);
        
    }
}
