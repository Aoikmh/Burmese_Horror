using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Watches where the player's camera is facing. Once the player looks
/// toward 'lookTarget' within 'angleThreshold' degrees, fires onPlayerLooked
/// once and stops checking.
///
/// Doesn't check anything until Arm() is called (e.g. from a dialogue's
/// onDialogueEnd) — so it can't fire before the story wants it to.
/// </summary>
public class LookTrigger : MonoBehaviour
{
    [Tooltip("The point the player needs to look toward — e.g. Grandpa's hidden position outside the window.")]
    public Transform lookTarget;

    [Tooltip("STRONGLY RECOMMENDED for First Person Controllers: drag your FPC's actual view Camera here directly. Leaving this empty relies on Camera.main, which can grab the wrong camera if your scene has more than one.")]
    public Transform playerCamera;

    [Tooltip("How close (in degrees) the player's view needs to be to lookTarget to count as 'looking at it'. Smaller = more precise aim required.")]
    public float angleThreshold = 25f;

    [Tooltip("Turn this on temporarily while tuning — logs the current angle to the Console a few times a second so you can see exactly how close you are.")]
    public bool debugShowAngle = false;

    [Tooltip("Fires once, the first time the player looks within range after being armed.")]
    public UnityEvent onPlayerLooked;

    private bool armed = false;
    private bool triggered = false;
    private bool warnedMissingCamera = false;
    private float debugLogTimer = 0f;

    public void Arm()
    {
        armed = true;
        triggered = false;

        Debug.Log("[LookTrigger] Armed. Watching for player to look at: " + (lookTarget != null ? lookTarget.name : "NOTHING — lookTarget is not assigned!"));
    }

    void Update()
    {
        if (!armed || triggered)
            return;

        if (lookTarget == null)
        {
            Debug.LogWarning("[LookTrigger] lookTarget is not assigned in the Inspector — cannot check angle.");
            return;
        }

        Transform cam = playerCamera != null ? playerCamera : (Camera.main != null ? Camera.main.transform : null);

        if (cam == null)
        {
            if (!warnedMissingCamera)
            {
                Debug.LogWarning("[LookTrigger] No camera found. Assign playerCamera directly in the Inspector.");
                warnedMissingCamera = true;
            }
            return;
        }

        Vector3 toTarget = (lookTarget.position - cam.position).normalized;
        float angle = Vector3.Angle(cam.forward, toTarget);

        if (debugShowAngle)
        {
            debugLogTimer += Time.deltaTime;
            if (debugLogTimer >= 0.3f)
            {
                debugLogTimer = 0f;
                Debug.Log("[LookTrigger] Current angle to " + lookTarget.name + ": " + angle.ToString("F1") + " degrees (using camera: " + cam.name + ") — need <= " + angleThreshold);
            }
        }

        if (angle <= angleThreshold)
        {
            triggered = true;
            armed = false;
            Debug.Log("[LookTrigger] Triggered! Player looked at " + lookTarget.name);
            onPlayerLooked?.Invoke();
        }
    }
}