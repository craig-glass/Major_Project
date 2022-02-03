using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlotBuildingHeights : MonoBehaviour
{
    [Range(1, 8)]
    public int octaves = 2;

    [Range(0, 1000)]
    public int xOffset = 0;

    [Range(0, 1000)]
    public int yOffset = 0;

    [Range(0.001f, 0.01f)]
    public float xScale = 0;

    [Range(0.001f, 0.01f)]
    public float yScale = 0;

    [Range(0.0f, 1.0f)]
    public float blackCutoff = 0f;

    [Range(0.0f, 1.0f)]
    public float darkGrayCutoff = 0f;

    [Range(0.0f, 1.0f)]
    public float mediumGrayCutoff = 0f;

    [Range(0.0f, 1.0f)]
    public float lightGrayCutoff = 0f;

    [Range(0.0f, 1.0f)]
    public float whiteCutoff = 0f;

    // OnValidate allows you to change values in the inspector and updtate or rerun the method to update the texture 
    private void OnValidate()
    {
        Texture2D texture = new Texture2D(1024, 1024);
        GetComponent<Renderer>().sharedMaterial.mainTexture = texture;

        float perlin;
        Color colour;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                perlin = fBM((x + xOffset) * xScale, (y + yOffset) * yScale, octaves); // Perlin likes tiny values, between 0 and 1

                colour = Color.black;

                if (perlin < blackCutoff) colour = Color.black;
                else if (perlin < darkGrayCutoff) colour = new Color(0.3f, 0.3f, 0.3f);
                else if (perlin < mediumGrayCutoff) colour = new Color(0.5f, 0.5f, 0.5f);
                else if (perlin < lightGrayCutoff) colour = new Color(0.7f, 0.7f, 0.7f);
                else colour = Color.white;


                texture.SetPixel(x, y, colour);
            }
        }
        texture.Apply();

    }

    public float fBM(float x, float y, int octaves)
    {
        float total = 0;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency);
            frequency *= 2;
        }

        return total / (float)octaves;
    }
}
