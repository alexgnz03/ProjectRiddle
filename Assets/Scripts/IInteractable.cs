
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    void ShowObject(GameObject showedObject);
    bool CanInteract();
}
