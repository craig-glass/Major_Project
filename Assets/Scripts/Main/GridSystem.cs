using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GameObject straight;
    public GameObject crossroads;
    public GameObject deadEnd;
    public GameObject corner;

    public int width = 220;
    public int depth = 220;

    // Start is called before the first frame update
    void Start()
    {
        for (int z = 0; z < depth; z += 20)
        {
            for (int x = 0; x < width; x += 20)
            {
                Vector3 pos = new Vector3(x, 0, z);
                GameObject r = Instantiate(crossroads, pos, Quaternion.identity);
                pos.z += 10;

                //r = Instantiate(straight, pos, Quaternion.identity);

                //pos.x += 10;
                //pos.z = z;

                //r = Instantiate(straight, pos, Quaternion.Euler(0, 90, 0));
            }
        }
    }


}
