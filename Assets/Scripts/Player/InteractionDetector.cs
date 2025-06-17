using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    public static IInteractable interactableInRange { get; private set; } = null;
    public GameObject interactionIcon;

    // Start is called before the first frame update
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("OnInteract");
        if (context.performed && !InventoryController.inventoryIsOpen)
        {
            interactionIcon.SetActive(false);

            if (interactableInRange != null)
            {
                // Asegúrate de que el GameObject sigue existiendo
                MonoBehaviour mb = interactableInRange as MonoBehaviour;
                if (mb != null && mb.gameObject != null)
                {
                    interactableInRange.Interact();
                }
                else
                {
                    interactableInRange = null;
                }
            }
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entró colision");
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }

    public void ClearCurrentInteractable(IInteractable interactable)
    {
        if (interactableInRange == interactable)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
