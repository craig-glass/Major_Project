using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCubes : MonoBehaviour
{
    public GameObject cubePrefab;

    float xAxis = 4.5f;
    float zAxis = 4.5f;
    Vector3 pos;
    float spacing = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateCubes(5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiateCubes(int quantity)
    {
        
        for (int x = 0; x < quantity; x++)
        {
            for (int y = 0; y < quantity; y++)
            {
                
                float yScale = Random.Range(1f, 4f);
                cubePrefab.transform.localScale = new Vector3(1f, yScale, 1f);
                pos = new Vector3(xAxis, yScale / 2, zAxis);
                Instantiate(cubePrefab, pos, Quaternion.identity);

                
                
                //cubePrefab.transform.position = pos + new Vector3(0f, yScale, 0f);
                xAxis -= spacing;
            }

            zAxis -= spacing;
            xAxis = 4.5f;
        }    
    }
}
