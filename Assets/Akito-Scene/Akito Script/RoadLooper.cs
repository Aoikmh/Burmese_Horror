using UnityEngine;

public class RoadLooper : MonoBehaviour
{
    public Transform player;
    public float speed = 10f;
    public float segmentLength = 40f;

    public Transform[] roadSegments;

    public bool looping = true;
    public bool moving = true;

    void Update()
    {
        if (!moving || player == null || roadSegments == null || roadSegments.Length == 0)
            return;

        // Move EVERY segment backward — this was the bug: the old code
        // moved RoadLooper's own transform instead of the segments.
        foreach (Transform segment in roadSegments)
        {
            segment.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        }

        if (!looping)
            return;

        foreach (Transform segment in roadSegments)
        {
            if (segment.position.z < player.position.z - segmentLength)
            {
                MoveSegmentToFront(segment);
            }
        }
    }

    void MoveSegmentToFront(Transform segment)
    {
        float furthestZ = float.MinValue;

        foreach (Transform otherSegment in roadSegments)
        {
            if (otherSegment.position.z > furthestZ)
            {
                furthestZ = otherSegment.position.z;
            }
        }

        segment.position = new Vector3(
            segment.position.x,
            segment.position.y,
            furthestZ + segmentLength
        );
    }

    public void StopDriving()
    {
        moving = false;
        looping = false;
    }
}