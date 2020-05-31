using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNAction {
    public float[] inputs;
    public float[] outputs;

    public NNAction (float[] inputs, float[] outputs) {
        this.inputs = (float[]) inputs.Clone ();
        this.outputs = (float[]) outputs.Clone ();
    }

    public NNAction (float[] inputs, float[] outputs, bool reward, Activation activation) {
        this.inputs = (float[]) inputs.Clone ();
        this.outputs = (float[]) outputs.Clone ();

        if (!reward) {
            FlipOutput (activation);
        } else {
            ImproveRewards (activation);
        }
    }

    void ImproveRewards (Activation activation) {
        for (int i = 0; i < outputs.Length; i++) {
            float op = outputs[i];
            float r = Random.value;
            if (r > 0.95f) {
                //Debug.Log (op + " kek got 1");
                outputs[i] = 1;
                continue;
            } else if (r < 0.05f) {
                //Debug.Log (op + " kek got 0");
                outputs[i] = 0;
                continue;
            }

            if (op > 0.5) {
                outputs[i] = 1;
            } else {
                outputs[i] = 0;
            }
        }
    }

    void FlipOutput (Activation activation) {
        for (int i = 0; i < outputs.Length; i++) {
            float op = outputs[i];
            float r = Random.value;
            if (r > 0.95f) {
                //Debug.Log (op + " kek got 1");
                outputs[i] = 1;
                continue;
            } else if (r < 0.05f) {
                //Debug.Log (op + " kek got 0");
                outputs[i] = 0;
                continue;
            }

            if (op > 0.5) {
                outputs[i] = 0;
            } else {
                outputs[i] = 1;
            }
        }
    }

}
