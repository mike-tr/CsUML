using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SimpleBrain {
    public class TrainingSet {
        private int[] layers;
        private float[][] cbiases;
        private float[][][] cweights;

        public float trainingSpeed = .1f;
        private float epocs;

        public int maxEpocs = 50;

        private float meanError = 1;
        private float slope = 0.999f;
        public float GetMeanError(float error) {
            meanError = meanError * slope + error * (1 - slope);
            return (error + meanError) * .5f;
        }

        public TrainingSet(int[] layers, float trainingSpeed, int maxEpocs) {
            epocs = 1;
            this.trainingSpeed = trainingSpeed;
            this.layers = layers;
            this.maxEpocs = maxEpocs;
            cbiases = new float[layers.Length][];
            for (int i = 0; i < layers.Length; i++) {
                float[] blayer = new float[layers[i]];
                cbiases[i] = blayer;
            }

            cweights = new float[layers.Length - 1][][];
            for (int i = 0; i < layers.Length - 1; i++) {
                float[][] nlayer = new float[layers[i + 1]][];
                for (int k = 0; k < layers[i + 1]; k++) {
                    float[] wlayer = new float[layers[i]];
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
            if(epocs == 0) {
                return;
            }
            for (int layer = 1; layer < layers.Length; layer++) {
                for (int neuron = 0; neuron < layers[layer]; neuron++) {
                    var bias = (cbiases[layer][neuron] * trainingSpeed);
                 
                    biases[layer][neuron] += bias * 2 / epocs;
                    cbiases[layer][neuron] = 0;
                    for (int neuron_am1 = 0; neuron_am1 < layers[layer - 1]; neuron_am1++) {
                        var weight = (cweights[layer - 1][neuron][neuron_am1] * trainingSpeed);

                        weights[layer - 1][neuron][neuron_am1] += weight * 2 / epocs;
                        cweights[layer - 1][neuron][neuron_am1] = 0;
                    }
                }
            }
            epocs = 0;
        }
    }
}
