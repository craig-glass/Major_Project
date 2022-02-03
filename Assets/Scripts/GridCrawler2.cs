using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

class RoadPiece : IEquatable<RoadPiece>
{
    public Vector3Int position;
    public GridCrawler2.RoadType type;
    public int yRotation;
    public GameObject road;

    public bool Equals(RoadPiece other)
    {
        return (position == other.position && type == other.type && yRotation == other.yRotation || 
            position == other.position && type == GridCrawler2.RoadType.STRAIGHT && other.type == GridCrawler2.RoadType.STRAIGHT
            && Mathf.Abs(yRotation - other.yRotation) == 180);
    }
}

public class GridCrawler2 : MonoBehaviour
{
    public GameObject crawler;
    public GameObject straight;
    public GameObject corner;
    public GameObject tJunction;
    public GameObject crossroad;
    public GameObject house;
    public GameObject shack;
    public GameObject lawn;

    public enum PieceType { ROAD, HOUSE, SHACK, LAWN };
    public Dictionary<Vector3Int, PieceType> citymap = new Dictionary<Vector3Int, PieceType>();

    int width = 500;
    int depth = 500;

    Vector3Int crawlerPos;
    Vector3 dir = new Vector3(0, 0, 1);
    Vector3 neutral = new Vector3(0, 0, 1);

    Vector3Int minDimensions = Vector3Int.zero;
    Vector3Int maxDimensions = Vector3Int.zero;

    public enum RoadType
    {
        STRAIGHT,
        CROSS,
        CORNER,
        TJUNCTION
    };

    List<RoadPiece> roadPieces = new List<RoadPiece>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= 5; i++)
        {
            Vector3Int mapKey = Vector3Int.RoundToInt(Vector3Int.RoundToInt(dir * -i));
            if (!citymap.ContainsKey(mapKey))
            {
                citymap.Add(mapKey, PieceType.ROAD);
            }
        }
    }

    int counter = 0;
    bool done = false;

    // Update is called once per frame
    void Update()
    {
        if (done) return;
        if (counter < 200)
        {
            Crawl();
            counter++;
        }
        else
        {
            FixRoads();
            Invoke("BuildHouses", 0.1f);
            done = true;
        }
    }

    void AddNoDuplicates(RoadPiece newPiece)
    {
        bool found = false;
        foreach (RoadPiece r in roadPieces)
        {
            if (r.Equals(newPiece))
            {
                found = true;
                break;
            }
        }
        if (!found)
            roadPieces.Add(newPiece);
        else
            DestroyImmediate(newPiece.road);
    }

    void FixRoads()
    {

        for (int i = 0; i < 5; i++)
        {
            Vector3Int mapKey = Vector3Int.RoundToInt(crawlerPos - Vector3Int.RoundToInt(dir * i));
            if (citymap.ContainsKey(mapKey))
            {
                citymap.Remove(mapKey);
            }
        }

        Lookup<Vector3Int, RoadPiece> lookup = (Lookup<Vector3Int, RoadPiece>)roadPieces.ToLookup(p => p.position, p => p);

        foreach (IGrouping<Vector3Int, RoadPiece> roadGroup in lookup)
        {
            if (roadGroup.Count() > 1)
            {
                bool hasCorner0 = false;
                bool hasCorner90 = false;
                bool hasCorner180 = false;
                bool hasCorner270 = false;

                bool hasStraight0 = false;
                bool hasStraight90 = false;
                bool hasStraight180 = false;
                bool hasStraight270 = false;

                foreach (RoadPiece r in roadGroup)
                {
                    if (r.yRotation == 0 && r.type == RoadType.CORNER) hasCorner0 = true;
                    if (r.yRotation == 90 && r.type == RoadType.CORNER) hasCorner90 = true;
                    if (r.yRotation == 180 && r.type == RoadType.CORNER) hasCorner180 = true;
                    if (r.yRotation == 270 && r.type == RoadType.CORNER) hasCorner270 = true;

                    if (r.yRotation == 0 && r.type == RoadType.STRAIGHT) hasStraight0 = true;
                    if (r.yRotation == 90 && r.type == RoadType.STRAIGHT) hasStraight90 = true;
                    if (r.yRotation == 180 && r.type == RoadType.STRAIGHT) hasStraight180 = true;
                    if (r.yRotation == 270 && r.type == RoadType.STRAIGHT) hasStraight270 = true;

                    DestroyImmediate(r.road);
                }

                if (hasStraight0 && hasStraight90 || hasStraight90 && hasStraight180 || hasStraight180 && hasStraight270 || hasStraight270 && hasStraight0 || hasCorner0 && hasCorner180 || hasCorner90 && hasCorner270)
                    Instantiate(crossroad, roadGroup.Key, Quaternion.identity);
                else if (hasCorner0 && hasCorner90 || hasCorner0 && hasStraight0 || hasCorner0 && hasStraight180 || hasCorner90 && hasStraight0 || hasCorner90 && hasStraight180)
                    Instantiate(tJunction, roadGroup.Key, Quaternion.identity);
                else if (hasCorner0 && hasCorner270 || hasCorner0 && hasStraight90 || hasCorner0 && hasStraight270 || hasCorner270 && hasStraight90 || hasCorner270 && hasStraight270)
                    Instantiate(tJunction, roadGroup.Key, Quaternion.Euler(0, -90, 0));
                else if (hasCorner90 && hasCorner180 || hasCorner90 && hasStraight90 || hasCorner90 && hasStraight270 || hasCorner180 && hasStraight90 || hasCorner180 && hasStraight270) 
                    Instantiate(tJunction, roadGroup.Key, Quaternion.Euler(0, 90, 0));
                else if (hasCorner180 && hasCorner270 || hasCorner180 && hasStraight0 || hasCorner180 && hasStraight180 || hasCorner270 && hasStraight0 || hasCorner270 && hasStraight180)
                    Instantiate(tJunction, roadGroup.Key, Quaternion.Euler(0, 180, 0));
                
            }
        }
    }

    void Crawl()
    {
        int randomTurn = UnityEngine.Random.Range(0, 3);
        float rot;
        GameObject go;
        RoadPiece newRoad;

        if (randomTurn == 0)
        {
            dir = Quaternion.Euler(0, -90, 0) * dir;
            rot = Vector3.SignedAngle(neutral, dir, this.transform.up) + 90;
            go = Instantiate(corner, crawlerPos, Quaternion.identity);
            go.transform.Rotate(0, rot, 0);

            newRoad = new RoadPiece { position = crawlerPos, type = RoadType.CORNER, yRotation = (int) Mathf.Round(go.transform.rotation.eulerAngles.y / 90) * 90, road = go };
        }
        else if (randomTurn == 1)
        {
            dir = Quaternion.Euler(0, 90, 0) * dir;
            rot = Vector3.SignedAngle(neutral, dir, this.transform.up) + 180;
            go = Instantiate(corner, crawlerPos, Quaternion.identity);
            go.transform.Rotate(0, rot, 0);

            newRoad = new RoadPiece { position = crawlerPos, type = RoadType.CORNER, yRotation = (int)Mathf.Round(go.transform.rotation.eulerAngles.y / 90) * 90, road = go };
        }
        else
        {
            rot = Vector3.SignedAngle(neutral, dir, this.transform.up);
            go = Instantiate(straight, crawlerPos, Quaternion.identity);
            go.transform.Rotate(0, rot, 0);

            newRoad = new RoadPiece { position = crawlerPos, type = RoadType.STRAIGHT, yRotation = (int)Mathf.Round(go.transform.rotation.eulerAngles.y / 90) * 90, road = go };
        }

        AddNoDuplicates(newRoad);

        Vector3Int straightPos = crawlerPos + Vector3Int.RoundToInt(dir * 10);

        rot = Vector3.SignedAngle(neutral, dir, this.transform.up);
        go = Instantiate(straight, straightPos, Quaternion.identity);
        go.transform.Rotate(0, rot, 0);

        newRoad = new RoadPiece { position = straightPos, type = RoadType.STRAIGHT, yRotation = (int)Mathf.Round(go.transform.rotation.eulerAngles.y / 90) * 90, road = go };

        AddNoDuplicates(newRoad);

        for (int i = 0; i <= 20; i++)
        {
            Vector3Int mapKey = Vector3Int.RoundToInt(crawlerPos + Vector3Int.RoundToInt(dir * i));
            if (!citymap.ContainsKey(mapKey))
            {
                citymap.Add(mapKey, PieceType.ROAD);
            }
        }

        crawlerPos += Vector3Int.RoundToInt(dir * 20);
        //if (crawlerPos.x > width || crawlerPos.x < 0 || crawlerPos.z > depth || crawlerPos.z < 0)
        //    crawlerPos -= Vector3Int.RoundToInt(dir * 20);
        crawler.transform.position = crawlerPos;

        if (minDimensions.x > crawlerPos.x) minDimensions.x = crawlerPos.x;
        if (minDimensions.z > crawlerPos.z) minDimensions.z = crawlerPos.z;
        if (maxDimensions.x < crawlerPos.x) maxDimensions.x = crawlerPos.x;
        if (maxDimensions.z < crawlerPos.z) maxDimensions.z = crawlerPos.z;
    }

    void BuildHouses()
    {
        for (int z = minDimensions.z - 10; z < maxDimensions.z + 10; z++)
        {
            for (int x = minDimensions.x - 10; x < maxDimensions.x + 10; x++)
            {
                Vector3Int pos = new Vector3Int(x, 0, z);

                GameObject[] buildings = { house, shack };

                PieceType pt;
                GameObject go;

                int rand = UnityEngine.Random.Range(0, 2);

                if (rand == 0)
                {
                    go = Instantiate(house, pos, Quaternion.identity);
                    pt = PieceType.HOUSE;
                }
                else
                {
                    go = Instantiate(shack, pos, Quaternion.identity);
                    pt = PieceType.SHACK;
                }


                RaycastHit hitUp = new RaycastHit();
                RaycastHit hitForward = new RaycastHit();
                RaycastHit hitBack = new RaycastHit();
                RaycastHit hitLeft = new RaycastHit();
                RaycastHit hitRight = new RaycastHit();

                if (Physics.Raycast(pos - new Vector3Int(0, 2, 0), Vector3.up, out hitUp, 3) ||
                    !Physics.Raycast(pos, go.transform.forward, out hitForward, 1) && !Physics.Raycast(pos, -go.transform.forward, out hitBack, 1) && !Physics.Raycast(pos, go.transform.right, out hitRight, 1) && !Physics.Raycast(pos, -go.transform.right, out hitLeft, 1))
                {
                    DestroyImmediate(go);
                    go = null;
                }

                if (go != null)
                {
                    if (hitForward.normal != Vector3.zero)
                    {
                        go.transform.LookAt(hitForward.point);
                    }
                    else if (hitBack.normal != Vector3.zero)
                    {
                        go.transform.LookAt(hitBack.point);
                    }
                    else if (hitLeft.normal != Vector3.zero)
                    {
                        go.transform.LookAt(hitLeft.point);
                    }
                    else if (hitRight.normal != Vector3.zero)
                    {
                        go.transform.LookAt(hitRight.point);
                    }

                    if (UnityEngine.Random.Range(0, 2) == 1)
                    {
                        if (!Physics.Raycast(go.transform.position - go.transform.forward, Vector3.up, out hitUp, 3))
                        {
                            go.transform.Translate(0, 0, -1);
                        }
                    }

                    Vector3Int mapKey = Vector3Int.RoundToInt(go.transform.position);
                    if (!citymap.ContainsKey(mapKey))
                    {
                        citymap.Add(mapKey, pt);
                    }
                }
            }
        }
        AddFillers();
     }

    void AddFillers()
    {
        List<Mesh> meshes = new List<Mesh>();
        List<Vector3> mPositions = new List<Vector3>();
        GameObject go = null;
        Material mat = null;
        
        for (int z = minDimensions.z - 10; z < maxDimensions.z + 10; z++)
        {
            for (int x = minDimensions.x - 10; x < maxDimensions.x + 10; x++)
            {
                Vector3Int mapKey = new Vector3Int(x, 0, z);
                if (!citymap.ContainsKey(mapKey))
                {
                    citymap.Add(mapKey, PieceType.LAWN);
                    go = Instantiate(lawn, mapKey, Quaternion.identity);

                    mat = go.GetComponent<MeshRenderer>().material;
                    MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();

                    foreach (MeshFilter mf in meshFilters)
                    {
                        meshes.Add(mf.mesh);
                        mPositions.Add(go.transform.position);
                    }
                    DestroyImmediate(go);
                }
            }
        }

        if (meshes.Count > 0)
        {
            GameObject combinedMesh = new GameObject("Combined Mesh");
            List<List<Mesh>> allMeshes = MeshTools.Split(meshes, 1000);
            List<List<Vector3>> allPositions = MeshTools.Split(mPositions, 1000);

            for ( int i = 0; i < allMeshes.Count; i++)
            {
                GameObject subMesh = new GameObject("SubMesh");
                subMesh.transform.parent = combinedMesh.transform;
                MeshRenderer mr = subMesh.AddComponent<MeshRenderer>();
                mr.material = mat;
                MeshFilter mf = subMesh.AddComponent<MeshFilter>();
                mf.mesh = MeshTools.MergeMeshes(allMeshes[i], allPositions[i]);
            }

            
        }
    }    
}
