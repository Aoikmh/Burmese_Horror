using UnityEngine;

public class ForkMover : MonoBehaviour
{
    public float speed = 10f;
    public bool moving = false;

    void Update()
    {
        if (!moving)
            return;

        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }

    public void StartMoving()
    {
        moving = true;
    }

    public void StopMoving()
    {
        moving = false;
    }
}