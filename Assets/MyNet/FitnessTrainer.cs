using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessTrainer {
    class NNActionList {
        public float fitness;

        FitnessTrainer parent;
        List<NNAction> actions = new List<NNAction> ();

        public NNActionList (FitnessTrainer parent, List<NNAction> actions, float fitness) {
            this.fitness = fitness;
            this.parent = parent;
            this.actions = actions;
        }

        public float Train (int batches, string setName) {
            return parent.TrainBrain (actions, batches, setName);
        }
    }

    public FitnessTrainer (SimpleBrain brain) {
        this.brain = brain;
    }

    public SimpleBrain brain;
    const int size = 10;
    List<NNActionList> trainingData = new List<NNActionList> ();
    int populated = 0;

    public void AddSet (List<NNAction> actions, float fitness) {
        NNActionList action = new NNActionList (this, actions, fitness);
        insertSet (action);

        //Debug.Log (trainingData.Count);
        //Debug.Log (trainingData[0].fitness);
        //action.Train (20, "lastSet");
        TrainAll (10);
        brain.ApplyTraining ();
        action.Train (2, "lastSet");
        brain.ApplyTraining ();
        TrainAll (10);
        brain.ApplyTraining ();
    }

    private void insertSet (NNActionList action) {
        if (trainingData.Count < 1) {
            trainingData.Add (action);
            return;
        }
        int i = 0;
        for (i = 0; i < trainingData.Count; i++) {
            //Debug.Log (trainingData[i].fitness + " , " + action.fitness);
            if (trainingData[i].fitness < action.fitness) {
                trainingData.Insert (i, action);
                break;
            }
        }
        if (i == trainingData.Count) {
            trainingData.Insert (i - 1, action);
        }
        if (trainingData.Count > size) {
            trainingData.RemoveAt (size);
        }
    }

    public double TrainAll (int batches) {
        double cost = 0;
        for (int i = 0; i < batches; i++) {
            for (int j = 0; j < trainingData.Count; j++) {
                var index = BiasedIndex (j);
                cost += trainingData[index].Train (3, "Datanum : " + index);
                index = BiasedIndex (j);
                cost += trainingData[j].Train (3, "Datanum : " + index);
            }
        }
        Debug.Log ("Training finished!! Overall cost " + cost / batches);
        return cost;
    }

    private int BiasedIndex (int range) {
        var i = Random.Range (0, range);
        var j = Random.Range (0, range);
        return i > j ? j : i;
    }

    public float TrainBrain (List<NNAction> trainingData, int repeat, string name) {
        double cost = 0;
        //int size = 0;

        //ShuffelIndexArray indexArray = new ShuffelIndexArray(trainingData.Count);
        for (int k = 0; k < repeat; k++) {
            for (int i = 0; i < trainingData.Count; i++) {
                var index = BiasedIndex (i);
                var data = trainingData[index];
                cost += brain.Train (data.inputs, data.outputs);

                index = BiasedIndex (i);
                data = trainingData[index];
                cost += brain.Train (data.inputs, data.outputs);

                data = trainingData[i];
                cost += brain.Train (data.inputs, data.outputs);
            }
            brain.ApplyTraining ();
        }
        cost /= repeat;
        //Debug.Log ("Training " + name + " Cost - " + cost + " Stats :: samples_total :" + size + " batches : " + repeat);
        return (float) cost;
    }
}
