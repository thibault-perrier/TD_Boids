using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteHouse : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Boid Has Crashed Onto The White House");
    }
}
