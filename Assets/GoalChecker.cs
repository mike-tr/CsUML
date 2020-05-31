using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalChecker : MonoBehaviour {
    private static List<GoalChecker> goals = new List<GoalChecker> ();
    // Start is called before the first frame update
    public Transform target;
    public Vector3 dir;

    public static GoalChecker active;
    void Start () {
        if (!target)
            target = CarController.instance.transform;
        goals.Add (this);
        distance = getDistance ();
        oldd = distance;
        if (active) {
            if (active.distance > distance && Vector2.Dot (dir, transform.position - target.position) > 0) {
                active = this;
            }
        } else {
            active = this;
        }
    }

    private float distance;
    private float oldd;
    public float getDistance () {
        distance = (target.position - transform.position).sqrMagnitude;
        return distance;
    }

    public float reward () {
        getDistance ();
        if (distance < 1.5f) {
            //Debug.Log (target.position);
            getNext ();
            GetBallGame.game.score += 0.5f;
            oldd = distance;
            return 0.5f;
        }
        //Debug.Log (oldd + " , " + distance);
        if (distance < oldd - 5f) {
            //Debug.Log (target.position + " nn");
            GetBallGame.game.score += 0.5f;
            oldd = distance;
            return 0.5f;
        }

        if (distance > oldd + 1f) {
            Debug.Log ("nani?!!");
            GetBallGame.game.score -= 0.5f;
            oldd = distance;
            return -0.5f;
        }
        return 0;
    }

    public static void getNext () {
        var n = active;
        var cdist = float.MaxValue;
        foreach (var goal in goals) {
            if (goal != active) {
                if (goal.getDistance () < cdist && Vector2.Dot (goal.dir, goal.transform.position - goal.target.position) > 0) {
                    n = goal;
                    cdist = goal.distance;
                    goal.oldd = goal.distance;
                }
            }
        }
        Debug.Log (active);
        active = n;
    }
}
