using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotVoronoi : MonoBehaviour
{
    [Range(1, 10)]
    public int locationCount = 5;

    private void OnValidate()
    {
        Texture2D texture = new Texture2D(1024, 1024);
        GetComponent<Renderer>().sharedMaterial.mainTexture = texture;

        Dictionary<Vector2Int, Color> locations = new Dictionary<Vector2Int, Color>();

        while (locations.Count < locationCount)
        {
            int x = Random.Range(0, texture.width);
            int y = Random.Range(0, texture.height);
            Color colour = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

            if (!locations.ContainsKey(new Vector2Int(x, y)))
            {
                locations.Add(new Vector2Int(x, y), colour);
                //texture.SetPixel(x, y, Color.black);
            }              
         
        }

        Color col = Color.white;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float distance = Mathf.Infinity;
                col = Color.white;

                foreach (KeyValuePair<Vector2Int, Color> val in locations)
                {
                    float distTo = Vector2Int.Distance(val.Key, new Vector2Int(x, y));

                    if (distTo < distance)
                    {
                        col = val.Value;
                        distance = distTo;
                    }
                }

                texture.SetPixel(x, y, col);
            }
        }
        texture.Apply();
    }
}
