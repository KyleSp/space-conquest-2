using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    private const float SPEED_MULTIPLIER = 50f;

    private GameObject star;

    void Start()
    {
        star = GameObject.FindGameObjectWithTag(Tag.STAR);
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, star.transform.position);
        float speed = 1 / Mathf.Sqrt(distance);
        transform.RotateAround(star.transform.position, Vector3.forward, Time.deltaTime * speed * SPEED_MULTIPLIER);
        
    }
}
