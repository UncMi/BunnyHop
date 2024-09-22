using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] public IInteractable interactable;

    public void Interact()
    {
        if (interactable != null)
        {
            interactable.Interact(this); 
        }
    }

    public void Interact2()
    {
        if (interactable != null)
        {
            interactable.Interact2(this);
        }
    }
}