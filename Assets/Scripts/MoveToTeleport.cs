using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTeleport : MonoBehaviour
{
    Vector3 startingPosition;

    [SerializeField]
    Vector3 endingPosition;

    [SerializeField]
    float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, endingPosition, step);

        if (transform.position == endingPosition)
        {
            transform.position = startingPosition;
        }
    }
}
