using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CarController : MonoBehaviour
{
    public static CarController instance;

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
    // Start is called before the first frame update

    public int maxTrainingData = 1000;
    void Start()
    {
        instance = this;
        //Goal.instance.changePos();
        spos = transform.position;
        dist = Vector2.Distance(transform.position, Goal.worldPos);
        pos = Goal.worldPos;
        rb = GetComponent<Rigidbody2D>();

        isHuman = human && isHuman;
        if (!isHuman) {
            brain = new SimpleBrain(layers.getLayers(), layers.getActivations(), .01f, 200, false);
        }
        Time.timeScale *= 10f;

        List<int> test = new List<int>();
        test.Add(1);
        test.Add(2);
        test.Add(3);
        test.Add(4);
        test.RemoveAt(0);

        foreach (var item in test) {
            Debug.Log(item);
        }
    }

    public void GetInputs(float[] inputs) {
        gas = inputs[0];
        if(gas < 0) {
            stop = -gas;
            gas = 0;
        } else {
            stop = 0;
        }

        right = inputs[1];
        if(right < 0) {
            left = -right;
            right = 0;
        } else {
            left = 0;
        }
    }

    public float[] GetBrainInputs() {
        float[] inputs = new float[layers.layers[0].neurons];
        int i = 0;
        float[] raycasts = new float[] { 0, 45, 90, 135, 180, 225, 270, 315 };
        for (; i < raycasts.Length; i++) {
            var x = raycasts[i];
            var dir = MathHelper.AngleToVector2(x + transform.eulerAngles.z);
            var start = (Vector2)transform.position + dir * .5f;
            RaycastHit2D hit = Physics2D.Raycast(start, dir, 3);
            if (hit.collider) {
                inputs[i] = hit.distance / 3f;
                Debug.DrawRay(start, dir * hit.distance, Color.red);
            } else {
                inputs[i] = 1f;
                Debug.DrawRay(start, dir * 3, Color.black);
            }
        }
        inputs[i + 1] = NFunctions.Tanh(rb.velocity.x);
        inputs[i + 2] = NFunctions.Tanh(rb.velocity.y);
        inputs[i + 3] = NFunctions.Tanh(rb.angularVelocity);
        inputs[i + 4] = transform.up.x;
        inputs[i + 5] = transform.up.y;

        return inputs;
    }

    public List<float[]> inputs = new List<float[]>();

    //Vector2 velocity = Vector2.zero;
    public void Update() {
        float[] input = null;
        if (isHuman) {
            GetInputs(human.GetInputs());
        } else {
            input = GetBrainInputs();         
            if(inputs.Count > maxSavedSteps) {
                inputs.RemoveAt(0);
            }
            inputs.Add(input);
            GetInputs(brain.Predict(input));
            time++;
            if(time > maxSavedSteps * 100) {
                time = 0;
                AddPenalty(1);
                ResetCar();
            }
        }

        rb.AddForce(transform.up * gas);
        if (rb.velocity.sqrMagnitude > 0.2f) {
            var remove = (Vector2)transform.up - rb.velocity.normalized;
            rb.AddForce(remove * 10);
        }
        
        rb.velocity *= .998f;
        float mag = rb.velocity.magnitude;
        if (stop > 0) {
            rb.velocity -= rb.velocity * stop * Time.deltaTime * sensetivity;          
        }
        
        rb.velocity = Quaternion.AngleAxis(rb.angularVelocity * Time.deltaTime, Vector3.forward) * rb.velocity;

        float turn = (left - right);
        if (Mathf.Abs(turn) > 0) {   
            mag = Mathf.Clamp(mag, .2f, 1f);
            if (mag < .5f)
                rb.AddForce(transform.up);
            rb.AddTorque(turn * sensetivity * mag);
        }     
        rb.angularVelocity *= .9f;
    }

    public void ResetCar() {
        transform.position = spos;
        transform.eulerAngles = Vector2.zero;
        rb.velocity *= 0;
        rb.angularVelocity = 0;
        time = 0;
        inputs = new List<float[]>();
        DistanceMeasure.ResetDM();
        //Goal.instance.changePos();
        //dist = Vector2.Distance(transform.position, Goal.worldPos);

        List<nnActionCost> jv = new List<nnActionCost>();
        jv.AddRange(badMoves);
        jv.AddRange(goodMoves);
        TrainBrain(jv, 25, "rv");
        TrainBrain(badMoves, 1, "b10");
        TrainBrain(goodMoves, 1, "g10");

        //goodMoves = new List<nnActionCost>();
        RemovePercentage(goodMoves, .25f);
        RemovePercentage(badMoves, .25f);
        //badMoves = new List<nnActionCost>();
    }

    public float TrainBrain(List<nnActionCost> trainingData, int repeat, string name) {
        double cost = 0;
        int size = 0;

        ShuffelIndexArray indexArray = new ShuffelIndexArray(trainingData.Count);
        for (int k = 0; k < repeat; k++) {        
            for (int i = 0; i < trainingData.Count * 0.67f; i++) {
                size++;
                int index = indexArray.GetNext();
                var data = trainingData[index];
                cost += brain.Train(data.inputs, data.desirableOutput);       
            }
            brain.ApplyTraining();
        }
        cost /= repeat;
        Debug.Log("Training " + name + " Cost - " + cost + " Stats :: samples_total :" + size + " batches : " + repeat);
        //erros = new List<float[]>();
        return (float)cost;
    }

    public void RemovePercentage(List<nnActionCost> targetList ,float v) {
        int num = (int)(targetList.Count * v);
        ShuffelIndexArray indexArray = new ShuffelIndexArray(targetList.Count - num);
        for (int i = 0; i < num; i++) {
            targetList.RemoveAt(indexArray.GetNext());
        }
    }
    public class nnActionCost {
        public float[] inputs;
        public float[] desirableOutput;
        public bool preserving = false;
    }
    List<nnActionCost> badMoves = new List<nnActionCost>();
    List<nnActionCost> goodMoves = new List<nnActionCost>();

    
    List<nnActionCost> mostSuccsesfull = new List<nnActionCost>();

    int c = 0;

    public void AddPenalty(float punishment) {
        time = 0;
        int count = 0;
        for (int i = inputs.Count - 1; i >= 0; i-- , count++) {
            if (count > penaltySteps)
                break;
            var input = inputs[i];
            nnActionCost action = new nnActionCost();
            action.inputs = input;
            var prediction = brain.Predict(input);
            var error = new float[prediction.Length];
            for (int j = 0; j < prediction.Length; j++) {
                var value = prediction[j];
                float r = value > 0 ? 0.01f : -0.01f;
                error[j] = Mathf.Clamp(value * 0.66f - Mathf.Clamp(1f / (value * 10f + r), -0.5f, 0.5f) - value * 0.34f, -1f, 1f);
            }
            action.desirableOutput = error;
            badMoves.Add(action);
        }
        inputs = new List<float[]>();
    }

    public void AddReward() {
        time = 0;
        foreach (var input in inputs) {
            nnActionCost action = new nnActionCost();
            action.inputs = input;
            
            var prediction = brain.Predict(input);

            var error = new float[prediction.Length];
            for (int j = 0; j < prediction.Length; j++) {
                error[j] = prediction[j];
            }
            action.desirableOutput = error;
            action.preserving = true;
            goodMoves.Add(action);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        var penalty = collision.relativeVelocity.magnitude;
        GetBallGame.game.score -= penalty;
        AddPenalty(2);
    }

    float sl = 0;
    int csl = 0;
    private void OnCollisionStay2D(Collision2D collision) {
        sl += Time.deltaTime;
        if(sl > 1) {
            csl++;
            sl = 0;
            AddPenalty(1);
        }

        if (csl > 5) {
            csl = 0;
            ResetCar();
        }
    }
}
