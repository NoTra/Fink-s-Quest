using UnityEngine;

namespace FinksQuest.Animation
{
    public class FloatAnimation : MonoBehaviour
    {
        void Update()
        {
            // Make the object float between -0.2 and -0.2696
            float sinY = Mathf.Sin(Time.time);
            float scaledY = Mathf.LerpUnclamped(-0.2696f, -0.2f, sinY);
            float clampedY = Mathf.Clamp(scaledY, -0.2696f, -0.2f);

            transform.localPosition = new Vector3(0f, clampedY, 0f);
        
        }
    }
}