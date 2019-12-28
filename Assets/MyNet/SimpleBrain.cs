using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SimpleBrain {
    public void ApplyTraining() {
        trainingAvg.Apply(biases, weights);
    }

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

    public float Train(float[] inputs, float[] outputs) {
        //get the neual net predictions
        float[] predictions = Predict(inputs);
        if (predictions == null) {
            return -1;
        }
        //create a cost derivitive vector
        float[][] dcost = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++) {
            dcost[i] = new float[layers[i]];
        }

        float cost = 0;
        for (int i = 0; i < predictions.Length; i++) {
            //calculate the db_cost for each output neuron and store it.
            var c = (outputs[i] - predictions[i]);
            dcost[layers.Length - 1][i] = 2 * c;
            cost += c*c;
        }

        //Calculate the nudge needed for each bias/weight in respect to the cost.
        for (int inputLayer = layers.Length - 2; inputLayer >= 0; inputLayer--) {      
            for (int outn = 0; outn < layers[inputLayer + 1]; outn++) {
                int outputLayer = inputLayer + 1;
                // we need to calculate d(C/B),
                // d(C/B) = d(Z/B) * d(A/Z) * d(C/A) , d(Z/B) = 1
                float dCA = dcost[outputLayer][outn];
                float dAZ = NFunctions.DerivativeActivation(activations[outputLayer], zneurons[outputLayer][outn], false);

                float bchange = dCA * dAZ;
                //biases[outputLayer][outn] += bchange * .1f;
                trainingAvg.AddChange(outputLayer, outn, bchange);

                for (int inpn = 0; inpn < layers[inputLayer]; inpn++) {
                    //we need to calculate in here the d(C/W),
                    // WC = d(Z/W)*d(A/Z)*d(C/A)
                    float dWC = neurons[inputLayer][inpn];
                    trainingAvg.AddChange(inputLayer, outn, inpn, dWC * bchange);

                    //weights[inputLayer][outn][inpn] += bchange * dWC * .1f;
                    dcost[inputLayer][inpn] += weights[inputLayer][outn][inpn] * bchange;           
                }
            }
        }

        if (trainingAvg.Epocs()) {
            trainingAvg.Apply(biases, weights);
        }
        return cost;
    }

    public void TrainTEST(float[] inputs, float[] outputs) {
        float[] predictions = Predict(inputs);
        if(predictions == null) {
            return;
        }
        
        string log = "Error - [ ";
        string plog = "Prediction - [ ";
        string ilog = "Inputs - [ ";
        string dlog = "desirable - [";
        for (int i = 0; i < predictions.Length; i++) {
            //costs[i] = Mathf.Pow(predictions[i] - outputs[i], 2);
            var error = 2 * (outputs[i] - predictions[i]);

            //error *= Mathf.Abs(error);
            //error = Mathf.Clamp(error, -.1f, .1f);
            
            
            Debug.Log(error + " ," + trainingAvg.GetMeanError(error));
            error = trainingAvg.GetMeanError(error);
            if (logging) {
                log += error + ", ";
                plog += predictions[i] + ", ";
                ilog += inputs[i] + ", ";
                dlog += outputs[i] + ", ";
            }

            bool inner = false;
            for (int inpLayer = layers.Length - 2; inpLayer >= 0; inpLayer--) {

                if (!inner) {               
                    //  where activation on output layer, and output neuron
                    float az = NFunctions.DerivativeActivation(activations[inpLayer + 1], zneurons[inpLayer + 1][i], false);
                    float bc = error * az;  // input(1) * Bf'(n) * error
                    //biases[inpLayer + 1][i] += bc;
                    trainingAvg.AddChange(inpLayer + 1, i, bc);

                    for (int inpNeuron = 0; inpNeuron < layers[inpLayer]; inpNeuron++) {
                        //Debug.Log(weights[inpLayer][i][inpNeuron] + " w : " + inpNeuron + " ,x : " + neurons[inpLayer][inpNeuron]);
                        float zw = zneurons[inpLayer][inpNeuron]; // the input for n;                                                          
                        float wc = zw * error * az; // input * Wf'(n) * error = "Slope" for minimizing the Cost(Error)!

                        //weights[inpLayer][i][inpNeuron] += wc;
                        trainingAvg.AddChange(inpLayer, i, inpNeuron, wc);
                        //trainingSum.Save(inpLayer, i, inpNeuron, wc, bc);
                    }
                    inner = true;
                } else {
                    var oLayer = inpLayer + 1;
                    for (int outNeuron = 0; outNeuron < layers[oLayer]; outNeuron++) {
                        //  where activation on output layer, and output neuron
                        float az = NFunctions.DerivativeActivation(activations[oLayer], zneurons[oLayer][outNeuron], false);
                        float bc = error * az;  // input(1) * Bf'(n) * error
                        trainingAvg.AddChange(oLayer, outNeuron, bc * .01f);
                        
                        //biases[oLayer][outNeuron] += bc;

                        for (int inpNeuron = 0; inpNeuron < layers[inpLayer]; inpNeuron++) {
                            //Debug.Log(weights[inpLayer][outNeuron][inpNeuron] + " w : " + inpNeuron + " ,x : " + neurons[inpLayer][inpNeuron]);

                            float zw = zneurons[inpLayer][inpNeuron]; // the input for n;                                         
                            float wc = zw * error * az; // input * Wf'(n) * error = "Slope" for minimizing the Cost(Error)!                         
                            //weights[inpLayer][outNeuron][inpNeuron] += wc;
                            trainingAvg.AddChange(inpLayer, outNeuron, inpNeuron, wc * 0.01f);
                        }
                    }
                }
                //inpLayer = input layer!
                // i = the y neuron
                // n = the x neuron
                
            }
        }

        if(trainingAvg.Epocs()) {
            trainingAvg.Apply(biases, weights);
        }
    }
}
