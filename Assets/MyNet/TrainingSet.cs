using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SimpleBrain {
    public class TrainingSet {
        private int[] layers;
        private float[][] cbiases;
        private float[][][] cweights;

        public float trainingSpeed = .1f;

        public TrainingSet(int[] layers, float trainingSpeed) {
            this.trainingSpeed = trainingSpeed;
            this.layers = layers;
            cbiases = new float[layers.Length][];
            for (int i = 0; i < layers.Length; i++) {
                float[] blayer = new float[layers[i]];
                for (int k = 0; k < layers[i]; k++) {
                    blayer[k] = Random.value - .5f;
                }
                cbiases[i] = blayer;
            }

            cweights = new float[layers.Length - 1][][];
            for (int i = 0; i < layers.Length - 1; i++) {
                float[][] nlayer = new float[layers[i + 1]][];
                for (int k = 0; k < layers[i + 1]; k++) {
                    float[] wlayer = new float[layers[i]];
                    for (int j = 0; j < layers[i]; j++) {
                        wlayer[j] = (Random.value - .5f) * 2f;
                    }
                    nlayer[k] = wlayer;
                }
                cweights[i] = nlayer;
            }
        }

        public void Save(int layer, int neuron, int targetNeuron, float cw, float cb) {
            cbiases[layer][neuron] += cb;
            cweights[layer][targetNeuron][neuron] += cw;
        }

        public void Apply(float[][] biases, float[][][] weights) {
            for (int layer = 0; layer < layers.Length; layer++) {
                for (int neuron = 0; neuron < layers[layer]; neuron++) {
                    biases[layer][neuron] += cbiases[layer][neuron] * trainingSpeed;
                    if (layer > 0) {
                        for (int neuron_am1 = 0; neuron_am1 < layers[layer - 1]; neuron_am1++) {
                            weights[layer][neuron][neuron_am1] += cweights[layer][neuron][neuron_am1] * trainingSpeed;
                        }
                    }
                }
            }
        }
    }
}
