using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float gas;
    private float right;
    private float left;
    private float stop;

    public float sensetivity = 5f;

    public HumanBrain human;
    public SimpleBrain brain;

    Rigidbody2D rb;

    public bool isHuman = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (isHuman) {
            isHuman = human != null;
        }
    }

    public void GetInputs(float[] inputs) {
        gas = inputs[0];
        right = inputs[1];
        left = inputs[2];
        stop = inputs[3];
    }

    //Vector2 velocity = Vector2.zero;
    public void Update() {
        if (isHuman) {
            GetInputs(human.GetInputs());
        }

        rb.AddForce(transform.up * gas);
        //velocity += (Vector2)transform.up * gas * Time.deltaTime * .01f * sensetivity;
        //float sx = Mathf.Abs(rb.velocity.x) * .05f;
        //rb.AddForce(-transform.up * sx);
        if (rb.velocity.sqrMagnitude > 1) {
            var remove = (Vector2)transform.up - rb.velocity.normalized;
            rb.AddForce(remove);
        }
        rb.velocity -= rb.velocity * stop * Time.deltaTime * sensetivity;
        rb.velocity *= .999f;
        

        rb.velocity = Quaternion.AngleAxis(rb.angularVelocity * Time.deltaTime, Vector3.forward) * rb.velocity;
        rb.AddTorque((left - right) * sensetivity);
        rb.angularVelocity *= .9f;

        //transform.position += (Vector3)velocity;
        //transform.Rotate(Vector3.forward, left - right);
    }
}
