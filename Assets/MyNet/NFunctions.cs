using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Activation {
    Sigmoid,
    Linear,
    Relu,
    Tanh,
}

public static class NFunctions 
{
    public static float Sigmoid(float x) {
        return 1 / (1 + Mathf.Exp(-x));
    }

    public static float InvSigmoid(float y) {
        return -1 / Mathf.Log((float)Math.E, 1 / y - 1);
    }

    public static float DerivativeSigmoid(float x, bool sigmoid = false) {
        return sigmoid ? x * (1 - x) : DerivativeSigmoid(Sigmoid(x), true);
    }

    public static float Relu(float x) {
        return x > 0 ? x : 0;
    }

    public static float Square(float x) {
        return x > 0 ? 1 : 0;
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

    public static void UseFunction(float[] output, Activation activation) {
        Func<float, float> function = ActivationEnumToFunc(activation);
        for (int i = 0; i < output.Length; i++) {
            output[i] = function(output[i]);
        }
    }

    public static Func<float, float> ActivationEnumToFunc(Activation activation) {
        switch (activation) {
            case Activation.Tanh:
                return Tanh;
            case Activation.Sigmoid:
                return Sigmoid;
            case Activation.Relu:
                return Relu;
        }
        return Linear;
    }

    public static float DerivativeActivation(Activation activation, float x, bool IsY) {
        switch (activation) {
            case Activation.Tanh:
                return IsY ? 1 - Mathf.Pow(x, 2) : 1 - Mathf.Pow(Tanh(x), 2);
            case Activation.Sigmoid:
                return DerivativeSigmoid(x, IsY);
            case Activation.Relu:
                return x > 0 ? 1 : 0;
        }
        return 1;
    }
}
