using UnityEngine;
using UnityEngine.Events;

public class ForkSequence : MonoBehaviour
{
    public RoadLooper roadLooper;
    public LinearMover forkMover;
    public Transform player;

    public float approachTime = 8f;

    [Tooltip("How far ahead of the player (in Z) the fork should appear when the sequence starts. Keep this bigger than forkMover's speed x approachTime so it's visible approaching from a distance, not already on top of the player.")]
    public float spawnDistanceAheadOfPlayer = 150f;

    [Header("On Arrived")]
    public UnityEvent onArrived;

    private bool active = false;
    private float timer = 0f;

    public void StartForkSequence()
    {
        if (active)
            return;

        active = true;
        timer = 0f;

        // Snap the fork to a position relative to the player RIGHT NOW,
        // instead of relying on wherever it was pre-placed in the Editor —
        // this is what keeps it lined up with the road no matter when dialogue ends.
        if (player != null)
        {
            Vector3 pos = forkMover.transform.position;
            pos.z = player.position.z + spawnDistanceAheadOfPlayer;
            forkMover.transform.position = pos;
        }

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