using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
    public static void SaveBrain (SimpleBrain brain, string name) {
        BinaryFormatter formatter = new BinaryFormatter ();
        var data = brain.GetData ();
        string path = Application.persistentDataPath + "/brains/";
        if (!Directory.Exists (path)) {
            Directory.CreateDirectory (path);
        }
        path += name + data.dname + ".nbrain";

        FileStream file = new FileStream (path, FileMode.Create);

        formatter.Serialize (file, data);
        file.Close ();

        Debug.Log ("saved at : " + path);
    }

    public static BrainData LoadBrain (string name, int[] layers, Activation[] activations) {
        string path = Application.persistentDataPath + "/brains/" + name +
            BrainData.CalcualteName (layers, activations) + ".nbrain";
        if (File.Exists (path)) {
            BinaryFormatter formatter = new BinaryFormatter ();
            FileStream file = new FileStream (path, FileMode.Open);

            BrainData data = formatter.Deserialize (file) as BrainData;
            file.Close ();
            return data;
        } else {
            //Debug.LogError ("Cant load brain, no such file");
            return null;
        }
    }
}