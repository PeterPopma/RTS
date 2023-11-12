using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float distance;
    float currentDistance;
    float timeDelayed;
    const float MOVE_DELAY = 0.9f;
    bool arrowReleased;
    Vector3 startPosition;
    float currentHeight;

    public float Distance { get => distance; set => distance = value; }

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; 
        currentHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeDelayed < MOVE_DELAY)
        {
            timeDelayed += Time.deltaTime;
            return;
        }
        else if (!arrowReleased)
        {
            arrowReleased = true;
            Sound.Instance.PlayArrowSound();
        }
        float movement = Time.deltaTime * 50;
        currentDistance += movement;
        transform.position += transform.forward * movement;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        if (currentDistance> distance)
        {
            Destroy(gameObject);
        }
        if (currentDistance < distance * .5)
        {
            currentHeight += 0.08f;
        }
        else
        {
            currentHeight -= 0.08f;
        }
        transform.Rotate(Time.deltaTime * 30f, 0, 0);
    }
}
