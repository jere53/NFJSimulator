using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VehiclePhysics;

public struct CalcularDistanciasParallelForJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float> posicionesA;
    [ReadOnly] public NativeArray<float> posicionB;

    public NativeArray<float> result;

    public void Execute(int index)
    {
        int i = index * 3;
        result[index] = Vector3.Distance(new Vector3(posicionesA[i], posicionesA[i+1], posicionesA[i+2]), 
            new Vector3(posicionB[0], posicionB[1], posicionB[2]));
    }
}