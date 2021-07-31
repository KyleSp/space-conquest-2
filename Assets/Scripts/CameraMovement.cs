using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const string AXIS_HORIZONTAL = "Horizontal";
    private const string AXIS_VERTICAL = "Vertical";

    public float maxMovementSpeed;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = maxMovementSpeed * Input.GetAxisRaw(AXIS_HORIZONTAL);
        float y = maxMovementSpeed * Input.GetAxisRaw(AXIS_VERTICAL);

        rb.velocity = new Vector3(x, y, 0);
    }

}
