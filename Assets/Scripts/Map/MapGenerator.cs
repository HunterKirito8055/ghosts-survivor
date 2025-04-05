using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<MapChunk> mapChunks = new List<MapChunk>();
    [SerializeField] private List<MapData> mapDataList = new List<MapData>();
    [SerializeField] private Transform playerTransform;
    [SerializeField] private MapChunk currentActiveMap;

    [SerializeField] private float rightDistance;
    [SerializeField] private float leftDistance;
    [SerializeField] private float upDistance;
    [SerializeField] private float downDistance;

    private NavMeshSurface meshSurface;
    public float mapSizeX;
    public float mapSizeZ;

    #region Unity Methods
    private void Awake()
    {
        // mapSizeX = currentActiveMap.GetComponent<Terrain>()..bounds.extents.x * 2f;
        //  mapSizeZ = currentActiveMap.GetComponent<MeshRenderer>().bounds.extents.z * 2f;
        meshSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        SetMapChunkBorders(currentActiveMap);
    }
    void LateUpdate()
    {
        CheckBoundsAndCreateMaps();
        CheckCurrentGround();
    }
    #endregion

    //public NavMeshData navmeshData;

    private void CheckCurrentGround()
    {
        if (Physics.Raycast(playerTransform.position, Vector3.down, out RaycastHit hit))
        {
            MapChunk map = hit.collider.GetComponent<MapChunk>();
            if (map != null)
            {
                if (map.MapID != currentActiveMap.MapID)
                {
                    currentActiveMap = map;
                    //currentActiveMap.PlaceProps();
                    AssignNewMapChunk();
                    SetMapChunkBorders(currentActiveMap);
                   // meshSurface.BuildNavMesh();
                   // meshSurface.UpdateNavMesh(meshSurface.navMeshData);
                }
            }
        }
    }

    private void AssignNewMapChunk()
    {
        for (int i = 0; i < currentActiveMap.StoreMaps.Count; i++)
        {
            if (currentActiveMap.StoreMaps[i].mapID != -1)
            {
                foreach (MapData item in mapDataList)
                {
                    if (item.creationSides.Exists(y => y == currentActiveMap.StoreMaps[i].side))
                    {
                        item.isMapChunksCreated = true;
                    }
                }
            }
        }
    }

    private void SetMapChunkBorders(MapChunk _mapchunk)
    {
        rightDistance = _mapchunk.right.position.x;
        leftDistance = _mapchunk.left.position.x;
        upDistance = _mapchunk.up.position.z;
        downDistance = _mapchunk.down.position.z;
    }

    private void CheckBoundsAndCreateMaps()
    {
        MapData MapRight = mapDataList.Find(x => x.crossedSide == MapChunkSides.Right);
        MapData MapUp = mapDataList.Find(x => x.crossedSide == MapChunkSides.Up);
        MapData MapLeftData = mapDataList.Find(x => x.crossedSide == MapChunkSides.Left);
        MapData MapDownData = mapDataList.Find(x => x.crossedSide == MapChunkSides.Down);

        //Check Right
        MapRight.isCrossed = playerTransform.position.x > rightDistance ? true : false;
        SpawnMapChunks(MapRight);

        //Check Left
        MapLeftData.isCrossed = playerTransform.position.x < leftDistance ? true : false;
        SpawnMapChunks(MapLeftData);

        //Check Up
        MapUp.isCrossed = playerTransform.position.z > upDistance ? true : false;
        SpawnMapChunks(MapUp);

        //Check Down
        MapDownData.isCrossed = playerTransform.position.z < downDistance ? true : false;
        SpawnMapChunks(MapDownData);
    }

    private void SpawnMapChunks(MapData _crossedMapChunksData)
    {
        if (_crossedMapChunksData.isCrossed)
        {
            //Clear the unSeen mapchunks
            MapData unSeenMaps = mapDataList.Find(x => !x.isCrossed && x.isMapChunksCreated);
            if (unSeenMaps != null)
            {
                unSeenMaps.isMapChunksCreated = false;
                currentActiveMap.ClearUnseenMapChunks(unSeenMaps.creationSides);
            }
            currentActiveMap.SpawnMapChunks(mapChunks, _crossedMapChunksData, mapSizeX, mapSizeZ);
        }
    }

}
public enum MapChunkSides
{
    LeftUp, Up, RightUp,
    Left, Right,
    LeftDown, Down, RightDown
}
[System.Serializable]
public class StoreMaps
{
    public MapChunkSides side;
    public MapChunk map;
    public int mapID;
}
[System.Serializable]
public class MapData
{
    public MapChunkSides crossedSide;
    public bool isCrossed;
    public bool isMapChunksCreated;
    public List<MapChunkSides> creationSides;// = new List<MapChunkSides>();
}
