using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;
    public float speed = 2f;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 target = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 offset = target - (Vector2)transform.position;
        offset.y = 0;
        transform.position += (Vector3)offset * Time.deltaTime * speed;

    }
}
