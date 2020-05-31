using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure : MonoBehaviour
{
    static int current = 0;
    static public Dictionary<int, Vector2> pm = new Dictionary<int, Vector2>();
    // Start is called before the first frame update
    public int Priority = 0;
    public bool reset = false;

    public static Vector2 GetTarget() {
        return pm[current];
    }

    public void Start() {
        pm.Add(Priority, transform.position);
    }
    public static void ResetDM() {
        current = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(current == Priority) {
            GetBallGame.game.score += Priority * 0.15f;
            CarController.instance.AddReward(Priority * 0.05f + 5f);
            current++;
            if (reset)
                current = 0;
        }
    }
}
