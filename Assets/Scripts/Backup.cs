using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backup : MonoBehaviour
{
    private float[,] _terrainBackup;
    
    private void Awake()
    {
        Debug.Log("Awake");
        CreateBackupForTerrain();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        RestoreTerrain();
    }
    
    private void CreateBackupForTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        var terrainData = terrain.terrainData;
        _terrainBackup = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    }
    
    private void RestoreTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        terrainData.SetHeights(0, 0, _terrainBackup);
    }
}
