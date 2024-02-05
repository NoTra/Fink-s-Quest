using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushableBehavior : MonoBehaviour
{
    private float collisionTime = 0f;
    public string axe;

    Coroutine movingCoroutine = null;
    Transform crate;
    Vector3 crateStartingPosition = Vector3.zero;

    Rigidbody playerRB;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        crate = transform.parent;
        audioSource = crate.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "Player")
        {
            playerRB = GameManager.Instance.Player.GetRigidbody();

            collisionTime += Time.deltaTime;
            if (collisionTime > 0.5f && movingCoroutine == null)
            {
                audioSource.Play();

                var crate = transform.parent;

                var crateRB = crate.GetComponent<Rigidbody>();
                crateRB.isKinematic = false;
                // playerRB.isKinematic = false;
                if (axe.Equals("x"))
                {
                    // Unfreeze axe X
                    crateRB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                } else
                {
                    // Unfreeze axe Z
                    crateRB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
                }

                crateStartingPosition = crate.position;
                movingCoroutine = StartCoroutine(MoveCrateAndPlayerTo(collider.transform.forward));
            }
        }
    }

    IEnumerator MoveCrateAndPlayerTo(Vector3 direction)
    {
        Debug.LogWarningFormat("MoveCrateAndPlayerTo {0}", direction);
        float duration = 0.8f;
        var crateRB = crate.GetComponent<Rigidbody>();

        float t = 0f;
        while (t < duration)
        {
            // crate.Translate(direction * Time.deltaTime, Space.World);
            crate.position = Vector3.Lerp(crateStartingPosition, crateStartingPosition + direction * 1.1f, t / duration);

            t += Time.deltaTime;

            yield return null;
        }

        movingCoroutine = null;
        crateRB.isKinematic = true;
        crateRB.constraints = RigidbodyConstraints.FreezeAll;

        audioSource.Stop();
        crateStartingPosition = Vector3.zero;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            collisionTime = 0f;

            var crate = transform.parent;
            crate.GetComponent<Rigidbody>().isKinematic = true;
            crate.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
