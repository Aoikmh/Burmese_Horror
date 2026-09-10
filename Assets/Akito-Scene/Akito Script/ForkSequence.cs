using UnityEngine;
using UnityEngine.Events;

public class ForkSequence : MonoBehaviour
{
    public RoadLooper roadLooper;
    public LinearMover forkMover;
    public Transform player;

    [Tooltip("How close the fork needs to get to the player before it counts as 'arrived' and stops.")]
    public float stopDistance = 10f;

    [Tooltip("Fallback safety timeout in case it somehow never gets close enough.")]
    public float approachTime = 15f;

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

        if (player != null)
        {
            float distance = Vector3.Distance(forkMover.transform.position, player.position);
            if (distance <= stopDistance)
            {
                Arrive();
                return;
            }
        }

        if (timer >= approachTime)
        {
            Arrive();
        }
    }

    public void Arrive()
    {
        if (!active)
            return;

        roadLooper.StopDriving();
        roadLooper.ShowOnlySegmentNearPlayer();
        forkMover.StopMoving();

        active = false;
        Debug.Log("ARRIVED AT FORK!");

        onArrived?.Invoke();
    }
}