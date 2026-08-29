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
        if (!moving)
            return;

        transform.Translate(Vector3.back * speed * Time.deltaTime);

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