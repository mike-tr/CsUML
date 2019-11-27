using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class NFunctions 
{
    public static float Sigmoid(float x) {
        return 1 / (1 + Mathf.Exp(-x));
    }

    public static float InvSigmoid(float y) {
        return -1 / Mathf.Log((float)Math.E, 1 / y - 1);
    }

    public static float divSigmoid(float x, bool sigmoid = false) {
        return sigmoid ? x * (1 - x) : divSigmoid(Sigmoid(x), true);
    }

    public static float Relu(float x) {
        return x > 0 ? x : 0;
    }

    public static float Linear(float x) {
        return x;
    }

    public static float Tanh(float x) {
        return (float)System.Math.Tanh(x);
    }

    public static void UseFunction(float[] output, Func<float, float> function) {
        for (int i = 0; i < output.Length; i++) {
            output[i] = function(output[i]);
        }
    }
}
