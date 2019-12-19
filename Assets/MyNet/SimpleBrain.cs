using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SimpleBrain {
  
    public float[] Predict(float[] inputs) {
        if(inputs.Length < layers[0]) {
            Debug.LogError("Expects " + layers[0] + " inputs, got only " + inputs.Length);
            return null;
        } else if (inputs.Length != layers[0]) {
            Debug.Log("got to much inputs!");
        }
        neurons[0] = inputs;
        zneurons[0] = inputs;
        for (int i = 0; i < layers.Length - 1; i++) {
            for (int j = 0; j < layers[i + 1]; j++) {
                neurons[i + 1][j] = biases[i + 1][j];
                for (int k = 0; k < layers[i]; k++) {
                    neurons[i + 1][j] += neurons[i][k] * weights[i][j][k];
                }
            }
            System.Array.Copy(neurons[i + 1], zneurons[i + 1], layers[i + 1]);
            if (i + 1 < layers.Length - 1) {
                NFunctions.UseFunction(neurons[i + 1], activations[i + 1]);
                continue;
            }
            NFunctions.UseFunction(neurons[i + 1], activations[i + 1]);
        }
        return neurons[layers.Length - 1];
    }

    public void Train(float[] inputs, float[] outputs) {
        float[] predictions = Predict(inputs);
        if(predictions == null) {
            return;
        }
        float[] costs = new float[predictions.Length];
        string log = "Error - [ ";
        string plog = "Prediction - [ ";
        string ilog = "Inputs - [ ";
        string dlog = "desirable - [";
        for (int i = 0; i < outputs.Length; i++) {
            costs[i] = Mathf.Pow(predictions[i] - outputs[i], 2);
            var error = 2 * (outputs[i] - predictions[i]);
            log += error + ", ";
            plog += predictions[i] + ", ";
            ilog += inputs[i] + ", ";
            dlog += outputs[i] + ", ";

            bool inner = false;
            for (int inpLayer = layers.Length - 2; inpLayer >= 0; inpLayer--) {

                if (!inner) {               
                    Debug.Log(inpLayer + " - output layers! - " + activations[inpLayer]);
                    //  where activation on output layer, and output neuron
                    float az = NFunctions.DerivativeActivation(activations[inpLayer + 1], zneurons[inpLayer + 1][i], false);
                    float bc = error * az;  // input(1) * Bf'(n) * error
                    biases[inpLayer + 1][i] += bc;

                    for (int inpNeuron = 0; inpNeuron < layers[inpLayer]; inpNeuron++) {
                        //Debug.Log(weights[inpLayer][i][inpNeuron] + " w : " + inpNeuron + " ,x : " + neurons[inpLayer][inpNeuron]);
                        float zw = zneurons[inpLayer][inpNeuron]; // the input for n;                                                          
                        float wc = zw * error * az; // input * Wf'(n) * error = "Slope" for minimizing the Cost(Error)!

                        weights[inpLayer][i][inpNeuron] += wc;
                        //trainingSum.Save(inpLayer, i, inpNeuron, wc, bc);
                    }
                    inner = true;
                } else {
                    Debug.Log(inpLayer + " - Inner layers! - " + activations[inpLayer]);
                    var oLayer = inpLayer + 1;
                    for (int outNeuron = 0; outNeuron < layers[oLayer]; outNeuron++) {
                        //  where activation on output layer, and output neuron
                        float az = NFunctions.DerivativeActivation(activations[oLayer], zneurons[oLayer][outNeuron], false);
                        float bc = error * az;  // input(1) * Bf'(n) * error
                        biases[oLayer][outNeuron] += bc;

                        for (int inpNeuron = 0; inpNeuron < layers[inpLayer]; inpNeuron++) {
                            //Debug.Log(weights[inpLayer][outNeuron][inpNeuron] + " w : " + inpNeuron + " ,x : " + neurons[inpLayer][inpNeuron]);

                            float zw = zneurons[inpLayer][inpNeuron]; // the input for n;                                         
                            float wc = zw * error * az; // input * Wf'(n) * error = "Slope" for minimizing the Cost(Error)!                         
                            weights[inpLayer][outNeuron][inpNeuron] += wc;                         
                        }
                    }
                }
                //inpLayer = input layer!
                // i = the y neuron
                // n = the x neuron
                
            }
        }
        log = log.Remove(log.Length - 2);
        plog = plog.Remove(plog.Length - 2);
        ilog = ilog.Remove(ilog.Length - 2);
        dlog = dlog.Remove(dlog.Length - 2);
        log += " ]";
        plog += " ]";
        dlog += " ]";
        ilog += " ]";
        Debug.Log(log + " , " + plog);
        Debug.Log(ilog + " , " + dlog);
    }
}
