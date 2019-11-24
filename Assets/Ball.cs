using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;

    public Vector3 velocity = Vector2.right;
    public Vector2 camBounds;
    public Vector2 offSet = Vector2.one;
    public float speed = 5f;
    [Range(0, 1f)]
    public float scale = 0.05f;

    public float revX = 0;
    public float revY = 0;

    public float revDelay = .2f;

    void Start()
    {
        cam = Camera.main;
        camBounds = OrthographicBounds(cam);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime * speed;
        if (revX < 0) {
            if (Mathf.Abs(transform.position.x * 2f) + offSet.x > camBounds.x) {
                velocity.x *= -1;
                revX = revDelay;
            }
        } else {
            revX -= Time.deltaTime;
        }
        if (revY < 0) {
            if (Mathf.Abs(transform.position.y * 2f) + offSet.y > camBounds.y) {
                velocity.y *= -1;
                revY = revDelay;
            }
        } else {
            revY -= Time.deltaTime;
        }
    }

    public static Vector2 OrthographicBounds(Camera camera) {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        return new Vector3(cameraHeight * screenAspect, cameraHeight);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        //Vector2 offset = collision.transform.position - transform.position;
        //Debug.Log("????");
        velocity.y *= -1.0f;
        //velocity.x *= 1.01f;
        if (speed < 15) {
            speed *= 1 + scale;
            scale *= .95f;
        }

        //Debug.Log("????");
    }

}
