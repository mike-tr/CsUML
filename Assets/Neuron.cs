using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron 
{
    public Neuron() {
        weight = Random.value * 2 - 1;
        bias = Random.value * 2 - 1;
    }

    public float weight;
    public float bias;

    public float Predict(float x) {
        return weight * x + bias;
    }

    public float Train(float x, float y) {
        var zl = Predict(x);
        var cost = (zl - y);
        cost *= cost;
        var error = y - zl;

        float ws = weight * x;
        float bs = (bias);

        float zw = x;
        float az = 1;
        float ca = 2 * (zl - y);

        //ws /= avg;
        //bs /= avg;


        Debug.Log("bias : " + bias + " | weight : " + weight );
        Debug.Log("error : " + error);

        weight -= zw * ca * az * .001f;
        bias -= ca * az * .001f;



        return Mathf.Pow((zl - y), 2);
    }
}
