using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActivate : PlayerSystem
{
    public void OnActivate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // On met la position du raycast au milieu du joueur
            Vector3 raycastStartPosition = Player.GetRigidbody().transform.position + new Vector3(0f, 0.5f, 0f);

            // Si on d�tecte un objet avec lequel on peut interagir (layer "Interactable") � moins de 0.5 devant le joueur
            RaycastHit hit;
            Debug.DrawRay(raycastStartPosition, Player.GetRigidbody().transform.forward * 0.5f, Color.red, 2f);
            if (Physics.Raycast(raycastStartPosition, Player.GetRigidbody().transform.forward, out hit, 0.5f, LayerMask.GetMask("Interactable")))
            {
                Debug.Log("On a touch� un objet interactable : " + hit.collider.gameObject.name);
                // On r�cup�re le composant Interactable de l'objet
                PushButton interactable = hit.collider.GetComponent<PushButton>();
                if (interactable != null)
                {
                    // On appelle la m�thode Toggle() de l'objet
                    interactable.Toggle();
                }
            }
        }
    }
}
