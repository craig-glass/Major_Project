using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCity : MonoBehaviour
{
    public int width = 100;
    public int depth = 100;

    public Material residential;
    public Material commercial;
    public Material industrial;

    // Start is called before the first frame update
    void Start()
    {
        MeshUtils.GenerateVoronoi(20, width, depth);

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3(x, 0, z);

                Renderer r = go.GetComponent<Renderer>();
                if (MeshUtils.voronoiMap[x, z] < 15)
                    r.material = residential;
                else if (MeshUtils.voronoiMap[x, z] < 18)
                    r.material = commercial;
                else if (MeshUtils.voronoiMap[x, z] < 20)
                    r.material = industrial;

                float perlin = MeshUtils.fBM(x * 0.005f, z * 0.005f, 5);

                int h = 1;
                if (perlin < 0.417f) h = 1;
                else if (perlin < 0.509f) h = 2;
                else if (perlin < 0.623f) h = 3;
                else if (perlin < 0.679f) h = 5;
                else h = 10;

                go.transform.localScale = new Vector3(1, h, 1);
                go.transform.Translate(0, h / 2.0f, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
