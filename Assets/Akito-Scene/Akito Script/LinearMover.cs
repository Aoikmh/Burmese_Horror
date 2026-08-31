using UnityEngine;

/// <summary>
/// Reusable "drive in a straight line forever" component.
/// Replaces both ForkMover and RoadMover — attach this to the fork object,
/// or anything else that just needs to move at a constant speed.
/// </summary>
public class LinearMover : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 direction = Vector3.back;

    [Tooltip("If true, starts moving as soon as the scene runs (RoadMover's old behavior). Leave false if something else (like ForkSequence) should call StartMoving().")]
    public bool moveImmediately = false;

    public bool moving = false;

    void Start()
    {
        if (moveImmediately)
            moving = true;
    }

    void Update()
    {
        if (!moving)
            return;

        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    public void StartMoving() => moving = true;
    public void StopMoving() => moving = false;
}