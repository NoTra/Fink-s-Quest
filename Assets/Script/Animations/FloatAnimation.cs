using UnityEngine;

namespace FinksQuest.Animation
{
    public class FloatAnimation : MonoBehaviour
    {
        [SerializeField] float from = -0.2f;
        [SerializeField] float to = -0.2696f;
        [SerializeField] float speed = 1f;

        [SerializeField] private AnimationCurve _curve;

        void Update()
        {
            // Lerp on y-axis 
            transform.localPosition = new Vector3(transform.localPosition.x, _curve.Evaluate(Mathf.Lerp(from, to, Mathf.PingPong(Time.time, speed))), transform.localPosition.z);
        }
    }
}