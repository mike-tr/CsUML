using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure : MonoBehaviour
{
    static int current = 0;
    // Start is called before the first frame update
    public int Priority = 0;
    public bool reset = false;
    public static void ResetDM() {
        current = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(current == Priority) {
            GetBallGame.game.score += Priority * 0.15f;
            CarController.instance.AddReward();
            current++;
            if (reset)
                current = 0;
        }
    }
}
