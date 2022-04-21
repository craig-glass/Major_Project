using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCubes : MonoBehaviour
{
    public GameObject cubePrefab;

    float xAxis = 4.5f;
    float zAxis = 4.5f;
    Vector3 pos;
    float spacing = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateCubes(500);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiateCubes(int quantity)
    {
        MeshUtils.GenerateVoronoi(20, quantity, quantity);

        for (int x = 10; x < quantity; x+= 20)
        {
            for (int y = 10; y < quantity; y+= 20)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);


                Renderer r = go.GetComponent<Renderer>();
                if (MeshUtils.voronoiMap[x, y] < 15)
                    r.material.color = Color.green;
                else if (MeshUtils.voronoiMap[x, y] < 18)
                    r.material.color = Color.red;
                else if (MeshUtils.voronoiMap[x, y] < 20)
                    r.material.color = Color.blue;

                float perlin = MeshUtils.fBM(x * 0.004f, y * 0.004f, 5);

                int h = 1;
                if (perlin < 0.417f) h = 10;
                else if (perlin < 0.509f) h = 20;
                else if (perlin < 0.623f) h = 30;
                else if (perlin < 0.679f) h = 50;
                else h = 60;

                //float yScale = Random.Range(10f, 40f);
                float yScale = h;
                go.transform.localScale = new Vector3(15f, yScale, 15f);
                go.transform.position = new Vector3(x, yScale / 2, y);

                //pos = new Vector3(x, yScale / 2, y);
                //Instantiate(cubePrefab, pos, Quaternion.identity);
                //xAxis -= spacing;
            }
            //zAxis -= spacing;
            //xAxis = 10.0f;
        }    
    }
}
