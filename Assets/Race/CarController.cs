using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
    public static CarController instance;

    FitnessTrainer trainer;
    public float fitness = 0;

    [SerializeField]
    private float gas;
    [SerializeField]
    private float right;
    [SerializeField]
    private float left;
    [SerializeField]
    private float stop;

    public float sensetivity = 5f;

    public HumanBrain human;
    public SimpleBrain brain;

    public NLayers layers;
    public int maxSavedSteps = 100;
    public int penaltySteps = 100;
    Rigidbody2D rb;

    public bool isHuman = true;
    public float time = 0;
    public float dist = 0;
    public Vector2 pos = Vector2.zero;
    private Vector3 spos = Vector3.zero;
    private Vector3 srot = Vector3.zero;
    // Start is called before the first frame update

    public int maxTrainingData = 1000;

    public string bname = "brain1";
    private int cycle = 0;
    void Start () {
        instance = this;
        //Goal.instance.changePos();
        spos = transform.position;
        srot = transform.eulerAngles;
        dist = Vector2.Distance (transform.position, Goal.worldPos);
        pos = Goal.worldPos;
        rb = GetComponent<Rigidbody2D> ();

        isHuman = human && isHuman;
        if (!isHuman) {
            var bl = SaveSystem.LoadBrain (bname, layers.getLayers (), layers.getActivations ());
            if (bl != null) {
                brain = new SimpleBrain (bl);
                Debug.Log (bl.cycles);
                Debug.Log (Application.persistentDataPath);
            } else {
                brain = new SimpleBrain (layers.getLayers (), layers.getActivations (), .08f, 20000, false);
            }
            //brain = new SimpleBrain (layers.getLayers (), layers.getActivations (), .08f, 20000, false);
        }
        trainer = new FitnessTrainer (brain);
        Time.timeScale *= 10f;

        List<int> test = new List<int> ();
        test.Add (1);
        test.Add (2);
        test.Add (3);
        test.Add (4);
        test.RemoveAt (0);

        foreach (var item in test) {
            Debug.Log (item);
        }
    }

    public void GetInputs (float[] inputs) {
        gas = inputs[0];
        gas = CVal (gas);
        stop = inputs[1];
        stop = CVal (stop);
        right = inputs[2];
        right = CVal (right);
        left = inputs[3];
        left = CVal (left);
    }

    private int CVal (float v) {
        return v > 0.5f ? 1 : 0;
    }

    public float[] inputl;
    public float[] GetBrainInputs () {
        int i = 0;
        float[] raycasts = new float[] { 0, 45, 90, 135, 180, 225, 270, 315 };
        inputl = new float[raycasts.Length + 7];
        for (; i < raycasts.Length; i++) {
            var x = raycasts[i];
            var dir = MathHelper.AngleToVector2 (x + transform.eulerAngles.z);
            var start = (Vector2) transform.position + dir * .5f;
            RaycastHit2D hit = Physics2D.Raycast (start, dir, 3);
            if (hit.collider) {
                inputl[i] = hit.distance / 3f;
                Debug.DrawRay (start, dir * hit.distance, Color.red);
            } else {
                inputl[i] = 1f;
                Debug.DrawRay (start, dir * 3, Color.black);
            }
        }
        inputl[i] = NFunctions.Tanh (rb.velocity.x);
        inputl[i + 1] = NFunctions.Tanh (rb.velocity.y);
        inputl[i + 2] = NFunctions.Tanh (rb.angularVelocity * Mathf.Deg2Rad * 0.5f);
        inputl[i + 3] = transform.up.x;
        inputl[i + 4] = transform.up.y;
        //var dist = (DistanceMeasure.GetTarget () - (Vector2) transform.position).normalized;
        var dist = (GoalChecker.active.transform.position - transform.position).normalized;
        inputl[i + 5] = dist.x;
        inputl[i + 6] = dist.y;
        //Debug.Log (dist);
        //Debug.Log (GoalChecker.active);
        return inputl;
    }

    public List<float[]> inputs = new List<float[]> ();

    //Vector2 velocity = Vector2.zero;
    public void Update () {
        float[] input = null;
        if (isHuman) {
            GetInputs (human.GetInputs ());
        } else {
            // Debug.Log (GoalChecker.active);
            input = GetBrainInputs ();
            if (inputs.Count > maxSavedSteps) {
                inputs.RemoveAt (0);
            }
            inputs.Add (input);
            GetInputs (brain.Predict (input));
            if (GoalChecker.active) {
                var r = GoalChecker.active.reward ();
                if (r > 0) {
                    AddReward (0.5f);
                } else if (r < 0) {
                    AddPenalty (-0.5f, 10);
                }
            }
            time++;
            //Debug.Log (time);
            if (time > maxSavedSteps * 10) {

                time = 0;
                AddPenalty (1);
                ResetCar ();
            }
        }

        rb.AddForce (transform.up * gas);
        if (rb.velocity.sqrMagnitude > 0.2f) {
            var remove = (Vector2) transform.up - rb.velocity.normalized;
            rb.AddForce (remove * 10);
        }

        rb.velocity *= .998f;
        float mag = rb.velocity.magnitude;
        if (stop > 0) {
            rb.velocity -= rb.velocity * stop * Time.deltaTime * sensetivity;
        }

        rb.velocity = Quaternion.AngleAxis (rb.angularVelocity * Time.deltaTime, Vector3.forward) * rb.velocity;

        float turn = (left - right);
        if (Mathf.Abs (turn) > 0) {
            mag = Mathf.Clamp (mag, .2f, 1f);
            if (mag < .5f)
                rb.AddForce (transform.up);
            rb.AddTorque (turn * sensetivity * mag);
        }
        rb.angularVelocity *= .9f;
    }

    public void ResetCar () {
        transform.position = spos;
        transform.eulerAngles = srot;
        rb.velocity *= 0;
        rb.angularVelocity = 0;
        time = 0;
        inputs = new List<float[]> ();
        DistanceMeasure.ResetDM ();
        //Goal.instance.changePos();
        //dist = Vector2.Distance(transform.position, Goal.worldPos);
        GoalChecker.active = null;
        GoalChecker.getNext ();

        var err = trainer.TrainBrain (bad, 10, "BAD");
        var err2 = trainer.TrainBrain (good, 10, "Good");

        Debug.Log ("reward cost matrix : " + err2);
        Debug.Log ("penalty cost matrix : " + err);

        Debug.Log (bad.Count);
        Debug.Log (good.Count);
        //trainer.TrainBrain (bad, 20, "BAD");

        trainer.AddSet (actions, fitness);

        actions = new List<NNAction> ();
        bad = new List<NNAction> ();
        good = new List<NNAction> ();
        fitness = 0;

        brain.cycles++;

        if (cycle % 10 == 0) {
            SaveSystem.SaveBrain (brain, bname);
        }
        cycle++;
        //badMoves = new List<nnActionCost>();
    }

    List<NNAction> actions = new List<NNAction> ();
    List<NNAction> bad = new List<NNAction> ();
    List<NNAction> good = new List<NNAction> ();

    public void AddPenalty (float punishment, int c = 20) {
        time *= 0.75f;
        int count = 0;
        inputs.Reverse ();
        float v = 0;
        var ed = 1 / c;
        foreach (var input in inputs) {
            if (count > c)
                break;
            var val = Random.value;
            if (val > count * c * 0.75f) {
                var prediction = brain.Predict (input);
                NNAction action = new NNAction (input, prediction, false, Activation.Sigmoid);
                actions.Add (action);
                bad.Add (action);
            }
            count++;
        }
        // if (inputs.Count > 7) {
        //     inputs = inputs.GetRange (0, 7);
        // }
        inputs = new List<float[]> ();
        fitness -= punishment * 0.1f;
    }

    public void AddReward (float reward) {
        //Debug.Log ("nani?" + inputs.Count);
        time = 0;
        fitness += reward;
        int count = 0;
        inputs.Reverse ();
        float v = 0;
        foreach (var input in inputs) {
            if (count > 4)
                break;
            var val = Random.value;
            if (val > v) {
                var prediction = brain.Predict (input);
                NNAction action = new NNAction (input, prediction, true, Activation.Sigmoid);
                actions.Add (action);
                good.Add (action);
                //bad.Add (action);
            }
            count++;
            v = val;
        }
        inputs.Reverse ();
    }
    private void OnCollisionEnter2D (Collision2D collision) {
        var penalty = collision.relativeVelocity.magnitude;
        GetBallGame.game.score -= penalty;
        AddPenalty (2);
    }

    float sl = 0;
    int csl = 0;
    private void OnCollisionStay2D (Collision2D collision) {
        sl += Time.deltaTime;
        if (sl > 1) {
            csl++;
            sl = 0;
            //AddPenalty(1);
        }

        if (csl > 5) {
            csl = 0;
            ResetCar ();
        }
    }
}
