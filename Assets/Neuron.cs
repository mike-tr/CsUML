using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron 
{
    int iterations = 0;
    public Neuron() {
        weight = Random.value * 2 - 1;
        bias = Random.value * 2 - 1;
    }

    public float weight;
    public float bias;
    public float neuron;

    public float Predict(float x) {
        neuron = weight * x + bias;
        return NFunctions.Sigmoid(neuron);
    }

    float dw = 0;
    float db = 0;
    float cost = 0;
    public float Train(float x, float y) {
        var p = Predict(x);
        var cost = (p - y);
        cost *= cost;
        var error = y - p;

        float zw = x; // the input for n;
        float az = NFunctions.DerivativeActivation(Activation.Sigmoid, p, true); // derivitive of F(n) => F'(n)
        float ca = 2 * (y - p); // desiried output;

        float wc = zw * ca * az; // input * F'(n) * error = "Slope" for minimizing the Cost(Error)!
        float bc = ca * az;  // input(1) * F'(n) * error

        wc = Mathf.Clamp(wc, -1, 1);
        bc = Mathf.Clamp(bc, -1, 1);

        //ws /= avg;
        //bs /= avg;


        //Debug.Log("bias : " + bias + " | weight : " + weight );
        //Debug.Log("error : " + error + " | wc : " + wc + ", bc : " + bc  + " iteration : " + iterations);

        dw += wc;
        db += bc;
        this.cost += cost;

        if(iterations % 10 == 0) {
            weight += dw * .05f;
            bias += db * .05f;
            db = 0;
            dw = 0;
            Debug.Log("Averate cost per 10 iterations - " + this.cost * 0.1f);
            this.cost = 0;
        }
        iterations++;
        return Mathf.Pow((p - y), 2);
    }
}
