using System.Collections;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    private void Start()
    {
        float randomValue = RandomValue();

        Debug.Log(randomValue);
    }

    public float RandomValue()
    {      
        return Random.value;
    }
}
