using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NLayer
{
    public int neurons = 1;
    public Activation activation = Activation.Linear;
}

[System.Serializable]
public class NLayers {
    public NLayer[] layers;
    public int[] getLayers() {
        int[] ilayers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++) {
            ilayers[i] = layers[i].neurons;
        }
        return ilayers;
    }

    public Activation[] getActivations() {
        Activation[] ilayers = new Activation[layers.Length];
        for (int i = 0; i < layers.Length; i++) {
            ilayers[i] = layers[i].activation;
        }
        return ilayers;
    }
}
