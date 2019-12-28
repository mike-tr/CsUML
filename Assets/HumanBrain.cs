using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanBrain", menuName = "Brains/HumanBrain", order = 1)]
public class HumanBrain : ScriptableObject
{
    [System.Serializable]
    public class ControllInput {
        public KeyCode key;
        [Range(0, 1f)]
        public float value = 1f;
    }

    public ControllInput[] inputs;
    // Start is called before the first frame update

    public float[] GetInputs() {
        float[] controllInputs = new float[inputs.Length];
        for (int i = 0; i < inputs.Length; i++) {
            var input = inputs[i];
            //Debug.Log(input.key + " " + Input.GetKeyDown(KeyCode.S));
            if (Input.GetKey(input.key)) {
                controllInputs[i] = input.value;
            }
        }
        return controllInputs;
    }
}
