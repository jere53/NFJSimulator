using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotSemaforo : Snapshot
{
    public int id;
    public int colorSemaforo;

    public SnapshotSemaforo(int id, int colorSemaforo)
    {
        this.id = id;
        this.colorSemaforo = colorSemaforo;
    }
    
    public SnapshotSemaforo(){}
}