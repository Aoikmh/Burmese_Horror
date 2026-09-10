using UnityEngine;

public class GenericItem : MonoBehaviour, IPickable, IInspectable, IHoldable
{
    [Header("Item Capabilities")]
    public bool canBePickedUp = true;
    public bool canBeInspected = true;
    public bool canBeHeld = false;


    [Header("Prompt Messages")]
    public string pickPrompt = "Press [E] to Pick Up";
    public string inspectPrompt = "Press [Q] to Inspect";
    public string holdPrompt = "Hold [C] to Carry";

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // --- IPickable Implementation ---
    public string GetPickUpMessage() => canBePickedUp ? pickPrompt : string.Empty;
    public void PickUp(PlayerInteraction player)
    {
        if (!canBePickedUp) return;

        gameObject.SetActive(false);
        Debug.Log($"Picked up {gameObject.name}");
    }

    public void Drop(Vector3 dropPosition)
    {
        EnablePhysics();
        transform.position = dropPosition;
    }

    // --- IInspectable Implementation ---
    public string GetInspectMessage() => canBeInspected ? inspectPrompt : string.Empty;
    public void StartInspect(Transform holdPoint)
    {
        if (!canBeInspected) return;

        DisablePhysics();
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void StopInspect()
    {
        EnablePhysics();
    }

    // --- IHoldable Implementation ---
    public string GetHoldMessage() => canBeHeld ? holdPrompt : string.Empty;
    public void Hold(Transform holdPoint)
    {
        if (!canBeHeld) return;

        DisablePhysics();
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Release(Vector3 dropPosition)
    {
        EnablePhysics();
        transform.position = dropPosition;

    }

    // --- Shared Physics Helpers ---
    private void DisablePhysics()
    {
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Collider[] allColliders = GetComponentsInChildren<Collider>();
        foreach (var col in allColliders)
        {
            col.enabled = false;
        }
    }

    public void EnablePhysics()
    {
        transform.SetParent(null);

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        Collider[] allColliders = GetComponentsInChildren<Collider>();
        foreach (var col in allColliders)
        {
            col.enabled = true;
        }
    }
}
