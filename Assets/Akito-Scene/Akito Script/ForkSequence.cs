using UnityEngine;
using UnityEngine.Events;

public class ForkSequence : MonoBehaviour
{
    public RoadLooper roadLooper;
    public LinearMover forkMover;

    public float approachTime = 8f;

    [Header("On Arrived")]
    [Tooltip("Fires when the fork sequence finishes. Hook this up to whatever should happen next — starting the wife-argument dialogue, for example. No code edits needed to change what happens next.")]
    public UnityEvent onArrived;

    private bool active = false;
    private float timer = 0f;

    public void StartForkSequence()
    {
        if (active)
            return;

        active = true;
        timer = 0f;

        forkMover.StartMoving();

        Debug.Log("Fork approaching!");
    }

    void Update()
    {
        if (!active)
            return;

        timer += Time.deltaTime;

        if (timer >= approachTime)
        {
            roadLooper.StopDriving();
            forkMover.StopMoving();

            active = false;
            Debug.Log("ARRIVED AT FORK!");

            onArrived?.Invoke();
        }
    }
}