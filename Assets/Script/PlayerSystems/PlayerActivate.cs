using UnityEngine;
using UnityEngine.InputSystem;

namespace FinksQuest.PlayerSystems
{
    public class PlayerActivate : PlayerSystem
    {
        public void OnActivate(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // On met la position du raycast au milieu du joueur
                Vector3 raycastStartPosition = _player.GetRigidbody().transform.position + new Vector3(0f, 0.5f, 0f);

                // Si on détecte un objet avec lequel on peut interagir (layer "Interactable") à moins de 0.5 devant le joueur
                RaycastHit hit;
                Debug.DrawRay(raycastStartPosition, _player.GetRigidbody().transform.forward * 0.5f, Color.red, 2f);
                if (Physics.Raycast(raycastStartPosition, _player.GetRigidbody().transform.forward, out hit, 0.5f, LayerMask.GetMask("Interactable")))
                {
                    // On récupère le composant Interactable de l'objet
                    Interactible.PushButton interactable = hit.collider.GetComponent<Interactible.PushButton>();
                    if (interactable != null)
                    {
                        // On appelle la méthode Toggle() de l'objet
                        interactable.Toggle();
                    }
                }
            }
        }
    }
}