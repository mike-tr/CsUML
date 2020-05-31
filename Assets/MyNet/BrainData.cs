using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrainData {

    public int[] layers;
    public int[] activations;
    // Layer, Neuron
    public float[][] neurons;
    public float[][] zneurons;
    public float[][] biases;
    // Layer, TargetNeuron, InpNeuron
    public float[][][] weights;

    public float trainingSpeed;
    public int epocs;
    public bool log;

    public string dname = "";

    public int cycles = 0;

    public Activation[] GetActivations () {
        Activation[] ret = new Activation[activations.Length];
        for (int i = 0; i < activations.Length; i++) {
            ret[i] = (Activation) (activations[i]);
        }
        return ret;
    }
    public BrainData (int[] layers, Activation[] activations, float[][] neurons, float[][] zneurons, float[][] biases,
        float[][][] weights, float trainingSpeed, int epocs, bool log, int cycles) {
        this.activations = new int[activations.Length];
        for (int i = 0; i < activations.Length; i++) {
            this.activations[i] = (int) activations[i];
        }
        this.layers = layers;
        this.neurons = neurons;
        this.zneurons = zneurons;
        this.biases = biases;
        this.weights = weights;
        this.trainingSpeed = trainingSpeed;
        this.epocs = epocs;
        this.log = log;
        this.cycles = cycles;

        dname = CalcualteName (layers, activations);
    }

    public static string CalcualteName (int[] layers, Activation[] activations) {
        int n = layers.Length;
        int v = 0;
        for (int i = 0; i < layers.Length; i++) {
            n += layers[i] * (i + 1);
            v += (int) activations[i] * i * i;
        }
        return n + "_" + v;
    }
}