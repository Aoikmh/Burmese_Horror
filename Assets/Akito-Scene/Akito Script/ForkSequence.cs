using UnityEngine;

public class ForkSequence : MonoBehaviour
{
    public RoadLooper roadLooper;
    public ForkMover forkMover;
    public GameObject choiceCanvas;

    public float approachTime = 8f;

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
            // Stop the road
            roadLooper.StopDriving();

            // Stop the fork
            forkMover.StopMoving();

            // Show choice
            choiceCanvas.SetActive(true);

            active = false;

            Debug.Log("ARRIVED AT FORK!");
        }
    }
}