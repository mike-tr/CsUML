using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffelIndexArray
{
    List<int> arr = new List<int>();
    int size = 1;
    public ShuffelIndexArray(int size) {
        RebuildIndexPool(size);
    }
    public void RebuildIndexPool(int size) {
        this.size = size;
        arr = new List<int>();
        for (int i = 0; i < size; i++) {
            arr.Add(i);        
        }
    }
    public int GetNext() {
        if(arr.Count <= 0) {
            RebuildIndexPool(size);
        }
        int index = (int)(Random.value * arr.Count);
        int v = arr[index];
        arr.RemoveAt(index);
        return v;
    }
}
