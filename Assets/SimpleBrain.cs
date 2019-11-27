using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBrain {
    private int[] layers;
    private float[][] neurons;
    private float[][] zneurons;
    private float[][] biases;
    private float[][][] weights;

    public SimpleBrain(int[] layers) {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++) {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitBiases();
        InitWeights();

        string test = "{";
        Debug.Log(weights);
        foreach(float[][] f in weights) {
            test += "[";
            foreach(float[] r in f) {
                test += " { ";
                foreach(float w in r) {
                    test += w + " ";
                }
                test += "}";
            }
            test += "]";
        }
        test += "}";
        Debug.Log(test);
    }

    public float[] GetWeights() {
        return null;
    }

    public void PrintNeurons() {
        string test = "[";
        foreach (float[] r in neurons) {
            test += " { ";
            foreach (float n in r) {
                test += n + " ";
            }
            test += "}";
        }
        test += " ]";
        Debug.Log(test);
    }

    public void PrintBiases() {
        string test = "[";
        foreach (float[] r in biases) {
            test += " { ";
            foreach (float n in r) {
                test += n + " ";
            }
            test += "}";
        }
        test += "]";
        Debug.Log(test);
    }

    public float[] Predict(float[] inputs) {
        if(inputs.Length != layers[0]) {
            Debug.LogError("Expects input the length of " + layers[0] + " but got instead input with the length of " + inputs.Length);
            return null;
        }
        neurons[0] = inputs;
        for (int i = 0; i < layers.Length - 1; i++) {
            for (int j = 0; j < layers[i + 1]; j++) {
                neurons[i + 1][j] = biases[i + 1][j];
                for (int k = 0; k < layers[i]; k++) {
                    neurons[i + 1][j] += neurons[i][k] * weights[i][j][k];
                }
            }
            System.Array.Copy(neurons[i + 1], zneurons[i + 1], layers[i + 1]);
            if (i + 1 < layers.Length - 1) {
                NFunctions.UseFunction(neurons[i + 1], NFunctions.Tanh);
                continue;
            }
            NFunctions.UseFunction(neurons[i + 1], NFunctions.Sigmoid);
        }
        return neurons[layers.Length - 1];
    }

    public void Train(float[] inputs, float[] outputs) {
        float[] predictions = Predict(inputs);
        string log = "Error - [";
        for (int i = 0; i < outputs.Length; i++) {
            log += Mathf.Pow(predictions[i] - outputs[i], 2) + " | " + (predictions[i] - outputs[i]) + " ";
        }
        log += " ]";
        //Debug.Log(log);
    }

    public void InitNeurons() {
        neurons = new float[layers.Length][];
        zneurons = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++) {
            float[] nlayer = new float[layers[i]];
            neurons[i] = nlayer;
            nlayer = new float[layers[i]];
            zneurons[i] = nlayer;
        }
    }

    public void InitBiases() {
        biases = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++) {
            float[] blayer = new float[layers[i]];
            for (int k = 0; k < layers[i]; k++) {
                blayer[k] = Random.value - .5f;
            }
            biases[i] = blayer;
        }
    }

    public void InitWeights() {
        weights = new float[layers.Length - 1][][];
        for (int i = 0; i < layers.Length - 1; i++) {
            float[][] nlayer = new float[layers[i + 1]][];
            for (int k = 0; k < layers[i + 1]; k++) {
                float[] wlayer = new float[layers[i]];
                for (int j = 0; j < layers[i]; j++) {
                    wlayer[j] = (Random.value - .5f) * 2f;
                }
                nlayer[k] = wlayer;
            }
            weights[i] = nlayer;
        }
    }
}
