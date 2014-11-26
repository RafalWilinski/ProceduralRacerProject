using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpponentsPool : MonoBehaviour 
{
    public Stack<Opponent> availableOpponents;
    public int lowCountAlert;
    public int availableOpponentsCount;

    void Start() {
        availableOpponents = new Stack<Opponent>();

        foreach (Transform t in transform) {
           availableOpponents.Push( t.GetComponent<Opponent>());
        }

        Debug.Log("Opponents Pool - Stack count: " + availableOpponents.Count);
    }

    public Opponent GetFirstAvailable() {
        if (availableOpponents.Count <= lowCountAlert) Debug.Log("Opponents Pool - Low amount of opponents in pool! Count: " + availableOpponents.Count);

        availableOpponentsCount = availableOpponents.Count;

        if (availableOpponents.Count > 0) return availableOpponents.Pop();
        else return null;
    }

    public void Return(Opponent opponent) {
        //Debug.Log("Opponents Pool - Returned!");
        availableOpponents.Push(opponent);
    }
}
