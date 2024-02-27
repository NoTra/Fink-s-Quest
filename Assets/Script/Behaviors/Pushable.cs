using System.Collections;
using UnityEngine;

using FinksQuest.Core;

namespace FinksQuest.Behavior
{
    public class Pushable : MonoBehaviour
    {
        private float collisionTime = 0f;
        public string axe;

        Coroutine movingCoroutine = null;
        Transform crate;
        Vector3 crateStartingPosition = Vector3.zero;
        Vector3 playerStartingPosition = Vector3.zero;

        Rigidbody playerRB;

        AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            crate = transform.parent;
            audioSource = crate.gameObject.GetComponent<AudioSource>();
        }

        private void OnTriggerStay(Collider collider)
        {
            if (!collider.CompareTag("Player"))
            {
                return;
            }

            Vector2 playerMovement = GameManager.Instance.Player._playerInput.actions["Move"].ReadValue<Vector2>();

            // Simplify movement
            if (Mathf.Abs(playerMovement.x) > Mathf.Abs(playerMovement.y))
            {
                playerMovement.y = 0;
            }
            else
            {
                playerMovement.x = 0;
            }

            if (
                collider.CompareTag("Player") && (
                    playerMovement.x != 0 && axe.Equals("x") ||
                    playerMovement.y != 0 && axe.Equals("z")
                )
            )
            {
                // On récupère le crate
                var crate = transform.parent;

                if (!CanPushInDirection(playerMovement))
                    return;

                playerRB = GameManager.Instance.Player.GetRigidbody();

                collisionTime += Time.deltaTime;
                if (collisionTime > 0.5f && movingCoroutine == null)
                {
                    audioSource.Play();

                    var crateRB = crate.GetComponent<Rigidbody>();
                    crateRB.isKinematic = false;
                    // playerRB.isKinematic = false;
                    if (axe.Equals("x"))
                    {
                        // Unfreeze axe X
                        crateRB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    }
                    else
                    {
                        // Unfreeze axe Z
                        crateRB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
                    }

                    crateStartingPosition = crate.position;
                    playerStartingPosition = playerRB.position;
                    movingCoroutine = StartCoroutine(MoveCrateAndPlayerTo(collider.transform.forward));
                }
            }
        }

        private bool CanPushInDirection(Vector2 playerMovement)
        {
            RaycastHit hit;
            if (
                Physics.Raycast(crate.position, Vector3.forward, out hit, 1f, LayerMask.GetMask("Wall")) && playerMovement.y > 0 || 
                Physics.Raycast(crate.position, Vector3.back, out hit, 1f, LayerMask.GetMask("Wall")) && playerMovement.y < 0 ||
                Physics.Raycast(crate.position, Vector3.right, out hit, 1f, LayerMask.GetMask("Wall")) && playerMovement.x > 0 ||
                Physics.Raycast(crate.position, Vector3.left, out hit, 1f, LayerMask.GetMask("Wall")) && playerMovement.x < 0
                )
            {
                return false;
            }

            return true;
        }

        IEnumerator MoveCrateAndPlayerTo(Vector3 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                direction.z = 0;
            }
            else
            {
                direction.x = 0;
            }

            float duration = 0.8f;
            var crateRB = crate.GetComponent<Rigidbody>();

            var playerRB = GameManager.Instance.Player.GetRigidbody();
            GameManager.Instance.Player._canMove = false;
            playerRB.velocity = Vector3.zero;

            crateRB.isKinematic = false;

            float t = 0f;
            while (t < duration)
            {
                // crate.Translate(direction * Time.deltaTime, Space.World);
                crate.position = Vector3.Lerp(crateStartingPosition, crateStartingPosition + direction * 0.8f, t / duration);
                playerRB.position = Vector3.Lerp(playerStartingPosition, playerStartingPosition + direction * 0.8f, t / duration);

                t += Time.deltaTime;

                yield return null;
            }

            crate.position = crateStartingPosition + direction * 0.8f;
            playerRB.position = playerStartingPosition + direction * 0.8f;

            GameManager.Instance.Player._canMove = true;
            movingCoroutine = null;
            crateRB.isKinematic = true;
            crateRB.constraints = RigidbodyConstraints.FreezeAll;

            audioSource.Stop();
            crateStartingPosition = Vector3.zero;
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                collisionTime = 0f;

                var crate = transform.parent;
                crate.GetComponent<Rigidbody>().isKinematic = true;
                crate.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
}