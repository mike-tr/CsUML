using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public static Goal instance;
    public static Vector2 worldPos;

    public float score = 1.25f;
    private float time = 1f;

    public bool dchange = false;
    // Start is called before the first frame update
    Camera cam;
    Collider2D coll;
    void Start()
    {
        instance = this;
        worldPos = transform.position;
        coll = GetComponent<Collider2D>();
        cam = Camera.main;
        changePos();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    public void changePos(int i = 0) {
        if (dchange)
            return;
        var pos = new Vector2(Random.value, Random.value) * .8f + Vector2.one * .1f;
        pos = cam.ViewportToWorldPoint(pos);
        if (Physics2D.OverlapCircle(pos, 2f) && i < 10) {
            changePos(i + 1);
        }
        worldPos = pos;
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        changePos();
        var car = collision.GetComponentInParent<CarController>();
        if (car) {
            var reward = score * .25f + (score * score) / time;
            GetBallGame.game.score += reward;
            //car.AddReward();
        }
        time = 1f;
    }
}
