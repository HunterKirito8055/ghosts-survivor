using AarquieSolutions.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using MathHelper;


public class MapChunk : MonoBehaviour
{
    [SerializeField] private int mapID;
    [SerializeField] private List<StoreMaps> storeMaps = new List<StoreMaps>();
    public Transform right;
    public Transform left;
    public Transform up;
    public Transform down;

    public int minProps = 10;
    public int maxProps = 16;
    public List<Transform> placeablePrefabs;

    #region properties
    public List<StoreMaps> StoreMaps
    {
        get => storeMaps;
    }
    public float offsetSpawning = 500;

    public int MapID
    {
        get
        {
            return mapID;
        }
        set
        {
            mapID = value;
        }
    }
    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        InitializeMapChunkSides();
    }
    private void OnEnable()
    {
        PlaceCorners();
        //PlaceProps();
    }

    public List<Transform> spawnCentres = new List<Transform>();
    public List<Transform> placedProps = new List<Transform>();
    // public void PlaceProps()
    // {
    //     int max = Random.Range(minProps, maxProps);
    //     if (placedProps != null && placedProps.Count > 0)
    //     {
    //         foreach (Transform placedProp in placedProps)
    //         {
    //             placedProp.gameObject.SetActive(false);
    //         }
    //     }
    //     int inc = max / 9;
    //     int j = 0;
    //     Vector3 currCentre = spawnCentres[j].position;
    //     for (int i = 0; i < max; i++)
    //     {
    //         if (i != 0 && i % inc == 0)
    //         {
    //             j++;
    //             if (j >= spawnCentres.Count)
    //             {
    //                 j = spawnCentres.Count - 1;
    //             }
    //             currCentre = spawnCentres[j].position;
    //         }
    //         Transform t = GetRandomProp;
    //         // Vector3 centre = transform.position + new Vector3(offsetSpawning, 0, offsetSpawning);
    //         // t.position = centre + (VectorHelper.GetDirectionAtAngle(Random.Range(0, 360)) * Random.Range(50,500));
    //         Vector3 centre = currCentre + new Vector3(offsetSpawning, 0, offsetSpawning);
    //         int checkTmer = 4;
    //         do
    //         {
    //             t.position = centre + (VectorHelper.GetDirectionAtAngle(Random.Range(0, 360)) * Random.Range(30, 125));
    //             checkTmer--;
    //         } while (!CheckMinDistance(t) && checkTmer > 0);
    //     }
    // }
    public List<Vector3> positions = new List<Vector3>();
    private void PlaceCorners()
    {
        if (placedProps.Count != 4)
        {
            foreach (Transform placeablePrefab in placeablePrefabs)
            {
                var go = Instantiate(placeablePrefab);
                go.SetParent(transform);
                go.gameObject.SetActive(true);
                go.localPosition = placeablePrefab.localPosition + new Vector3(offsetSpawning, 0, offsetSpawning);
                placedProps.Add(go);
                positions.Add(go.localPosition);
            }
        }
        positions.Shuffle();
        for (int i = 0; i < placedProps.Count; i++)
        {
            placedProps[i].localPosition = positions[i];
        }
    }
    bool CheckMinDistance(Transform a)
    {
        foreach (Transform item in placedProps)
        {
            if (a != item)
                if (Vector3.Distance(a.position, item.position) < 120)
                {
                    return false;
                }
        }
        return true;
    }
    // Transform GetRandomProp
    // {
    //     get
    //     {
    //         Transform t = null;
    //         if (placedProps != null)
    //         {
    //             if (placedProps.Count > 0)
    //             {
    //                 t = placedProps.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
    //             }
    //         }
    //         if (t == null)
    //         {
    //             t = Instantiate(placeablePrefabs[Random.Range(0, placeablePrefabs.Count)], this.transform);
    //             if (placedProps == null)
    //             {
    //                 placedProps = new List<Transform>();
    //             }
    //             placedProps.Add(t);
    //         }
    //         t.gameObject.SetActive(true);
    //         return t;
    //     }
    // }

    private void InitializeMapChunkSides()
    {
        storeMaps = new List<StoreMaps>();
        for (int i = 0; i < Enum.GetValues(typeof(MapChunkSides)).Length; i++)
        {
            storeMaps.Add(new StoreMaps
            {
                side = (MapChunkSides)i,
                mapID = -1
            });
        }
    }
    public void SpawnMapChunks(List<MapChunk> mapChunks, MapData _mapData, float _mapSizeX, float _mapSizeZ)
    {
        for (int i = 0; i < _mapData.creationSides.Count; i++)
        {
            //Check if MapChunk already exists
            if (storeMaps.Exists(x => x.side == _mapData.creationSides[i] && x.mapID != -1))
            {
                continue;
            }

            Vector3 pos = transform.position;
            switch (_mapData.creationSides[i])
            {
                case MapChunkSides.LeftUp:
                    pos += (Vector3.left * _mapSizeX) + (Vector3.forward * _mapSizeZ);
                    break;
                case MapChunkSides.Up:
                    pos += (Vector3.forward) * _mapSizeZ;
                    break;
                case MapChunkSides.RightUp:
                    pos += (Vector3.right * _mapSizeX) + (Vector3.forward * _mapSizeZ);
                    break;
                case MapChunkSides.Left:
                    pos += (Vector3.left) * _mapSizeX;
                    break;
                case MapChunkSides.Right:
                    pos += (Vector3.right) * _mapSizeX;
                    break;
                case MapChunkSides.LeftDown:
                    pos += (Vector3.left * _mapSizeX) - (Vector3.forward * _mapSizeZ);
                    break;
                case MapChunkSides.Down:
                    pos += (Vector3.back) * _mapSizeZ;
                    break;
                case MapChunkSides.RightDown:
                    pos += (Vector3.right * _mapSizeX) - (Vector3.forward * _mapSizeZ);
                    break;
                default:
                    break;
            }

            //New Map Chunk
            MapChunk mapChunk = mapChunks.FirstOrDefault(x => (x.MapID != this.MapID && !storeMaps.Exists(storeMap => storeMap.mapID == x.MapID)));
            mapChunk.SetMapPosition(pos);

            //Store Map Chunk
            int storeMapIndex = storeMaps.FindIndex(x => x.side == _mapData.creationSides[i]);
            storeMaps[storeMapIndex].mapID = mapChunk.MapID;
            storeMaps[storeMapIndex].map = mapChunk;
        }
        _mapData.isMapChunksCreated = true;
        AssignStoreMapChunksData();
    }

    private void SetMapPosition(Vector3 pos)
    {
        transform.position = pos;
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    private void AssignStoreMapChunksData()
    {
        for (int i = 0; i < storeMaps.Count; i++)
        {
            //Store Map ids
            if (storeMaps[i].mapID != -1)
            {
                //All Stored Maps
                StoreMaps rightMap = storeMaps.Find(x => x.side == MapChunkSides.Right);
                StoreMaps leftMap = storeMaps.Find(x => x.side == MapChunkSides.Left);
                StoreMaps upMap = storeMaps.Find(x => x.side == MapChunkSides.Up);
                StoreMaps downMap = storeMaps.Find(x => x.side == MapChunkSides.Down);
                StoreMaps rightUpMap = storeMaps.Find(x => x.side == MapChunkSides.RightUp);
                StoreMaps leftUpMap = storeMaps.Find(x => x.side == MapChunkSides.LeftUp);
                StoreMaps rightDownMap = storeMaps.Find(x => x.side == MapChunkSides.RightDown);
                StoreMaps leftdownMap = storeMaps.Find(x => x.side == MapChunkSides.LeftDown);


                StoreMaps[] _storeMaps = new StoreMaps[0];
                switch (storeMaps[i].side)
                {
                    case MapChunkSides.LeftUp:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.RightDown,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Right,
                                map = upMap.map,
                                mapID = upMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Down,
                                map = leftMap.map,
                                mapID = leftMap.mapID
                            }
                        };
                        break;
                    case MapChunkSides.Up:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.Left,
                                map = leftUpMap.map,
                                mapID = leftUpMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Right,
                                map = rightUpMap.map,
                                mapID = rightUpMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Down,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.LeftDown,
                                map = leftMap.map,
                                mapID = leftMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.RightDown,
                                map = rightMap.map,
                                mapID = rightMap.mapID
                            }
                        };
                        break;
                    case MapChunkSides.RightUp:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.LeftDown,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Down,
                                map = rightMap.map,
                                mapID = rightMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Left,
                                map = upMap.map,
                                mapID = upMap.mapID
                            }
                        };
                        break;
                    case MapChunkSides.Left:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.Up,
                                map = leftUpMap.map,
                                mapID = leftUpMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.RightUp,
                                map = upMap.map,
                                mapID = upMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Right,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.RightDown,
                                map = downMap.map,
                                mapID = downMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Down,
                                map = leftdownMap.map,
                                mapID = leftdownMap.mapID
                            }
                        };
                        break;
                    case MapChunkSides.Right:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.Left,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Up,
                                map = rightUpMap.map,
                                mapID = rightUpMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Down,
                                map = rightDownMap.map,
                                mapID = rightDownMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.LeftDown,
                                map = downMap.map,
                                mapID = downMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.LeftUp,
                                map = upMap.map,
                                mapID = upMap.mapID
                            }
                        };
                        break;
                    case MapChunkSides.LeftDown:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.RightUp,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Up,
                                map = leftMap.map,
                                mapID = leftMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Right,
                                map = downMap.map,
                                mapID = downMap.mapID
                            }
                        };
                        break;
                    case MapChunkSides.Down:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.Left,
                                map = leftdownMap.map,
                                mapID = leftdownMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Right,
                                map = rightDownMap.map,
                                mapID = rightDownMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Up,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.LeftUp,
                                map = leftMap.map,
                                mapID = leftMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.RightUp,
                                map = rightMap.map,
                                mapID = rightMap.mapID
                            }
                        };
                        break;
                    case MapChunkSides.RightDown:
                        _storeMaps = new StoreMaps[]
                        {
                            new StoreMaps
                            {
                                side = MapChunkSides.LeftUp,
                                map = this,
                                mapID = this.MapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Left,
                                map = downMap.map,
                                mapID = downMap.mapID
                            },
                            new StoreMaps
                            {
                                side = MapChunkSides.Up,
                                map = rightMap.map,
                                mapID = rightMap.mapID
                            }
                        };
                        break;
                    default:
                        break;
                }
                storeMaps[i].map.AssignChunksData(_storeMaps);
            }
        }
    }
    private void AssignChunksData(StoreMaps[] _storeMaps)
    {
        for (int i = 0; i < _storeMaps.Length; i++)
        {
            int storeMapIndex = storeMaps.FindIndex(x => x.side == _storeMaps[i].side);
            storeMaps[storeMapIndex].mapID = _storeMaps[i].mapID;
            storeMaps[storeMapIndex].map = _storeMaps[i].map;
        }
    }

    public void ClearUnseenMapChunks(List<MapChunkSides> mapChunkSides)
    {
        for (int i = 0; i < mapChunkSides.Count; i++)
        {
            int storeMapIndex = storeMaps.FindIndex(x => x.side == mapChunkSides[i]);
            storeMaps[storeMapIndex].mapID = -1;
            if (storeMaps[storeMapIndex].map != null)
            {
                storeMaps[storeMapIndex].map.InitializeMapChunkSides();
                storeMaps[storeMapIndex].map.gameObject.SetActive(false);
                storeMaps[storeMapIndex].map = null;
            }
        }
    }
}
