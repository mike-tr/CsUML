using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SimpleBrain {
    public class TrainingSet {
        private int[] layers;
        private float[][] cbiases;
        private float[][][] cweights;

        public float trainingSpeed = .1f;
        private int epocs;

        public int maxEpocs = 50;

        public TrainingSet(int[] layers, float trainingSpeed, int maxEpocs) {
            this.trainingSpeed = trainingSpeed;
            this.layers = layers;
            this.maxEpocs = maxEpocs;
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

        public void AddChange(int layer, int TargetNeuron, float biasChange) {
            cbiases[layer][TargetNeuron] += biasChange;
        }
        public void AddChange(int layer, int targetNeuron, int weight, float weightChange) {
            cweights[layer][targetNeuron][weight] += weightChange;
        }

        public bool Epocs() {
            epocs++;
            return epocs > maxEpocs;
        }

        public void Apply(float[][] biases, float[][][] weights, bool check = false) {
            if (check && !Epocs()) {
                return;
            }
            epocs = 1;
            for (int layer = 0; layer < layers.Length; layer++) {
                for (int neuron = 0; neuron < layers[layer]; neuron++) {
                    biases[layer][neuron] += cbiases[layer][neuron] * trainingSpeed;
                    cbiases[layer][neuron] = 0;
                    if (layer > 0) {
                        for (int neuron_am1 = 0; neuron_am1 < layers[layer - 1]; neuron_am1++) {
                            weights[layer - 1][neuron][neuron_am1] += cweights[layer - 1][neuron][neuron_am1] * trainingSpeed;
                            cweights[layer - 1][neuron][neuron_am1] = 0;
                        }
                    }
                }
            }
        }
    }
}
