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

    SimpleBrain brain;

    void Start() {
        Activation[] activations = new Activation[layers.Length];
        for (int i = 0; i < activations.Length; i++) {
            activations[i] = i == activations.Length - 1 ? Activation.Linear : Activation.Tanh;
        }
        brain = new SimpleBrain(layers, activations, false);

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Transform point = Instantiate(circle).transform;
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            point.position = pos;

            points.Add(point);
        }

        DrawGraph();
        Train(500);
        brain.ApplyTraining();

        Vector2 sc = new Vector2(.1f, 0);
        sc.y = brain.Predict(new float[] { sc.x })[0];
        Debug.Log(cam.ViewportToWorldPoint(sc));

        sc.x = 0.5f;
        sc.y = brain.Predict(new float[] { sc.x })[0];
        Debug.Log(cam.ViewportToWorldPoint(sc));

        sc.x = 0.9f;
        sc.y = brain.Predict(new float[] { sc.x })[0];
        Debug.Log(cam.ViewportToWorldPoint(sc));
        brain.PrintWeights();
        brain.PrintBiases();
    }

    const int size = 10;
    void DrawGraph() {
        line.positionCount = size + 1;
        float x = 0;
        for (int i = 0; i <= size; i++, x += 1f / size) {
            var prediction = brain.Predict(new float[] { x });
            //Vector2 point = new Vector2(x, (prediction[0] + 1) * .5f);
            var y = prediction[0];
            Vector2 point = new Vector2(x, y);
            Vector2 pos = cam.ViewportToWorldPoint(point);

            //Debug.Log(y);
            line.SetPosition(i, pos);
        }
        //line.SetPosition(0, cam.(Vector2.one));
    }

    void Train(int size) {
        var max = points.Count;
        if (size > max)
            size = max;
        for (int i = 0; i < size; i++) {
            int random = Random.Range(0, max);
            var target = PosToViewPort(points[random].position);
            float[] input = new float[] { target.x };
            float[] output = new float[] { target.y };
            brain.Train(input, output);
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
