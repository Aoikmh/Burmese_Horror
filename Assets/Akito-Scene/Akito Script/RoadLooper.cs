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

    // Hides every road segment except whichever one is currently
    // closest to the player, so leftover straight segments don't
    // clutter up the fork junction visually.
    public void ShowOnlySegmentNearPlayer()
    {
        if (player == null || roadSegments == null || roadSegments.Length == 0)
            return;

        Transform nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Transform segment in roadSegments)
        {
            float dist = Mathf.Abs(segment.position.z - player.position.z);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = segment;
            }
        }

        foreach (Transform segment in roadSegments)
        {
            segment.gameObject.SetActive(segment == nearest);
        }
    }
}