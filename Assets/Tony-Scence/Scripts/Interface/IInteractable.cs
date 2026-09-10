using UnityEngine;

public interface IInteractable { }

public interface IPrimaryInteractable : IInteractable
{
    string GetInteractMessage();
    void Interact(PlayerInteraction player);
}

public interface IPickable : IInteractable
{
    string GetPickUpMessage();
    void PickUp(PlayerInteraction player);
    void Drop(Vector3 dropPosition);
}

public interface IInspectable : IInteractable
{
    string GetInspectMessage();
    void StartInspect(Transform holdPoint);
    void StopInspect();
}

public interface IHoldable : IInteractable
{
    string GetHoldMessage();
    void Hold(Transform holdPoint);
    void Release(Vector3 dropPosition);
}
