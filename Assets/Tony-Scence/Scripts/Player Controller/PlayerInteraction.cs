using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private HUD hud;


    [Header("Controls")]
    [SerializeField] private KeyCode pickKey = KeyCode.E;
    [SerializeField] private KeyCode holdKey = KeyCode.C;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private KeyCode inspectKey = KeyCode.Q;

    private GameObject nearbyObject;
    private IPickable currentInventoryItem;
    private IInspectable currentInspectingItem;
    private IHoldable currentHoldingItem;
    private bool isBusy = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (holdPoint == null)
        {
            holdPoint = transform.Find("HoldPoint") ?? transform;
        }
    }

    void Update()
    {
        if (!isBusy)
        {
            HandleInputs();
        }
    }

    private void HandleInputs()
    {
        // Pick Up to Inventory
        if (Input.GetKeyDown(pickKey) && nearbyObject != null && currentHoldingItem == null)
        {
            if (nearbyObject.TryGetComponent<IPickable>(out var pickable) && !string.IsNullOrEmpty(pickable.GetPickUpMessage()))
            {
                // TODO: Add Item to Inventory Logic
                StartCoroutine(PickupRoutine(pickable));
            }
        }

        // Drop Item
        if (Input.GetKeyDown(dropKey) && currentInventoryItem != null)
        {
            Vector3 dropPos = transform.position + transform.forward * 1.5f;
            currentInventoryItem.Drop(dropPos);
            currentInventoryItem = null;
        }

        // Inspect Toggle
        if (currentInspectingItem != null)
        {
            if (Input.GetKeyDown(inspectKey))
            {
                SetAnimationBool("inspectItem", false);
                currentInspectingItem.StopInspect();
                currentInspectingItem = null;
            }
            return;
        }

        if (Input.GetKeyDown(inspectKey) && nearbyObject != null)
        {
            if (nearbyObject.TryGetComponent<IInspectable>(out var inspectable) && !string.IsNullOrEmpty(inspectable.GetInspectMessage()))
            {
                currentInspectingItem = inspectable;
                SetAnimationBool("inspectItem", true);
                currentInspectingItem.StartInspect(holdPoint);
                hud?.CloseAll();
                ClearNearbyState();
            }
            // else if (inventoryItem != null && inventoryItem.CanBeInspected())
            // {
            //     // TODO: Extract Item from Inventory Logic
            //     inspectingItem = inventoryItem;
            //     isInspectingFromInventory = true;
            //     StartInspecting();
            //     inventoryItem = null;
            // }
        }


        // Hold & Release
        if (currentHoldingItem != null)
        {
            if (Input.GetKeyUp(holdKey))
            {
                Vector3 dropPos = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
                SetAnimationBool("holdSmallItem", false);
                currentHoldingItem.Release(dropPos);
                currentHoldingItem = null;
            }
            return;
        }

        if (Input.GetKeyDown(holdKey) && nearbyObject != null)
        {
            if (nearbyObject.TryGetComponent<IHoldable>(out var holdable) && !string.IsNullOrEmpty(holdable.GetHoldMessage()))
            {
                currentHoldingItem = holdable;
                SetAnimationBool("holdSmallItem", true);
                currentHoldingItem.Hold(holdPoint);
                hud?.CloseAll();
                ClearNearbyState();
            }
            // else if (inventoryItem != null && inventoryItem.CanBeHeld())
            // {
            //     // TODO: Extract Item from Inventory Logic
            //     heldItem = inventoryItem;
            //     isHeldFromInventory = true;
            //     StartHolding();
            //     inventoryItem = null;
            // }
        }

    }

    private IEnumerator PickupRoutine(IPickable pickable)
    {
        isBusy = true;
        ClearNearbyState();
        hud?.CloseAll();
        TriggerAnimation("pickItem");

        yield return new WaitForSeconds(0.7f);

        pickable.PickUp(this);
        isBusy = false;
    }

    // --- TRIGGER DETECTION & HUD UPDATES ---
    private void OnTriggerStay(Collider other)
    {
        if (isBusy || currentHoldingItem != null || currentInspectingItem != null) return;

        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        Transform itemTransform = (interactable as Component)?.transform;

        // If nearbyObject was cleared, re-assign it immediately
        if (other.CompareTag("InnerDetectionZone") && nearbyObject == null)
        {
            nearbyObject = itemTransform?.gameObject;
            DisplayHUDMessage(itemTransform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBusy) return;

        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        Transform itemTransform = (interactable as Component)?.transform;

        if (other.CompareTag("OuterDetectionZone"))
        {
            hud?.ShowWhiteDot(itemTransform);
        }
        else if (other.CompareTag("InnerDetectionZone"))
        {
            nearbyObject = itemTransform?.gameObject;
            DisplayHUDMessage(itemTransform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        Transform itemTransform = (interactable as Component)?.transform;

        if (other.CompareTag("OuterDetectionZone"))
        {
            hud?.CloseAll();
        }
        else if (other.CompareTag("InnerDetectionZone"))
        {
            hud?.ShowWhiteDot(itemTransform);
        }

        if (nearbyObject != null && itemTransform != null && itemTransform.gameObject == nearbyObject)
        {
            ClearNearbyState();
        }
    }

    private void DisplayHUDMessage(Transform target)
    {
        if (hud == null || nearbyObject == null) return;

        List<string> messages = new List<string>();

        void TryAdd<T>(System.Func<T, string> getMsg)
        {
            if (nearbyObject.TryGetComponent<T>(out var comp))
            {
                string msg = getMsg(comp);
                if (!string.IsNullOrEmpty(msg)) messages.Add(msg);
            }
        }

        TryAdd<IPrimaryInteractable>(x => x.GetInteractMessage());
        TryAdd<IPickable>(x => x.GetPickUpMessage());
        TryAdd<IInspectable>(x => x.GetInspectMessage());
        TryAdd<IHoldable>(x => x.GetHoldMessage());

        if (messages.Count > 0)
        {
            hud.OpenMessagePanel(string.Join("\n", messages), target);
        }
    }

    public void ClearNearbyState()
    {
        nearbyObject = null;
    }

    // --- ANIMATION HELPERS ---
    private void TriggerAnimation(string triggerName)
    {
        if (animator != null) animator.SetTrigger(triggerName);
    }

    private void SetAnimationBool(string boolName, bool value)
    {
        if (animator != null) animator.SetBool(boolName, value);
    }
}
