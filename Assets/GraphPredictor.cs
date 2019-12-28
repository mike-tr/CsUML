using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPredictor : MonoBehaviour
{
    // Start is called before the first frame update
    public int[] layers;

    public GameObject circle;

    public List<Transform> points = new List<Transform>();
    public LineRenderer line;
    private Camera cam;

    private int xFactor = 10;

    SimpleBrain brain;

    void Start() {
        xFactor = layers[0];
        Activation[] activations = new Activation[layers.Length];
        for (int i = 0; i < activations.Length; i++) {
            activations[i] = i == activations.Length - 1 ? Activation.Linear : Activation.Linear;
        }
        brain = new SimpleBrain(layers, activations, 0.05f, 2500, false);

        cam = Camera.main;
        //float[] input = new float[xFactor];
        //for (int j = 0; j < xFactor; j++) {
        //    input[j] = Mathf.Pow(0, j);
        //}
        //brain.TrainR(input, new float[] { 0 } );

        //Debug.Log(brain.getWeight(1, 0, 3));

        brain.PrintWeights();
        brain.PrintBiases();
    }

    // Update is called once per frame
    int cycle = 0;
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Transform point = Instantiate(circle).transform;
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            point.position = pos;

            points.Add(point);
        }

        DrawGraph();
        cycle++;
        Train(10);
        brain.ApplyTraining();
        if (cycle > 10) {
            
            cycle = 0;
        }
        

        //Vector2 sc = new Vector2(.1f, 0);
        //sc.y = brain.Predict(new float[] { sc.x })[0];
        //sc.y = Mathf.Clamp(sc.y, -1, 1);
        //if (float.IsNaN(sc.y)) {
        //    sc.y = -2;
        //}
        ////Debug.Log(cam.ViewportToWorldPoint(sc));

        //sc.x = 0.5f;
        //sc.y = brain.Predict(new float[] { sc.x })[0];
        //sc.y = Mathf.Clamp(sc.y, -1, 1);
        //if (float.IsNaN(sc.y)) {
        //    sc.y = -2;
        //}
        ////Debug.Log(cam.ViewportToWorldPoint(sc));

        //sc.x = 0.9f;
        //sc.y = brain.Predict(new float[] { sc.x })[0];
        //sc.y = Mathf.Clamp(sc.y, -1, 1);
        //if(float.IsNaN(sc.y)) {
        //    sc.y = -2;
        //}
        //Debug.Log(cam.ViewportToWorldPoint(sc));
        brain.PrintWeights();
        brain.PrintBiases();
    }

    const int size = 100;
    void DrawGraph() {
        line.positionCount = size + 1;
        float x = 0;
        for (int i = 0; i <= size; i++, x += 1f / size) {

            float[] xs = new float[xFactor];
            for (int j = 0; j < xFactor; j++) {
                xs[j] = Mathf.Pow(x, j);
            }
            var prediction = brain.Predict(xs);
            //Vector2 point = new Vector2(x, (prediction[0] + 1) * .5f);
            var y = prediction[0];
            Vector2 point = new Vector2(x, y);
            point.y = Mathf.Clamp(point.y, -2, 2);
            if (float.IsNaN(point.y)) {
                point.y = -2;
            }
            Vector2 pos = cam.ViewportToWorldPoint(point);

            //Debug.Log(y);
            line.SetPosition(i, pos);
        }
        //line.SetPosition(0, cam.(Vector2.one));
    }

    void Train(int size) {
        var max = points.Count;
        if (max == 0)
            return;
        for (int i = 0; i < size; i++) {
            int random = Random.Range(0, max);
            var target = PosToViewPort(points[random].position);
            var x = target.x;
            float[] input = new float[xFactor];
            for (int j = 0; j < xFactor; j++) {
                input[j] = Mathf.Pow(x, j);
            }
            float[] output = new float[] { target.y };
            brain.Train(input, output);
            //brain.ApplyTraining();
        }
    }

    public Vector2 PosToScreenRatio(Vector2 pos) {
        return pos / (OrthographicBounds(cam) * .5f);
    }

    public Vector2 PosToViewPort(Vector2 pos) {
        return cam.WorldToViewportPoint(pos);
    }

    public static Vector2 OrthographicBounds(Camera camera) {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        return new Vector3(cameraHeight * screenAspect, cameraHeight);
    }
}
