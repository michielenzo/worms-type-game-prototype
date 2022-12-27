using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backup : MonoBehaviour
{
    private float[,] _terrainHeightsBackup;
    private float[,,] _terrainAlphasBackup;

    private void Awake()
    { 
        CreateBackupForTerrain();
    }

    private void OnApplicationQuit()
    {
        RestoreTerrain();
    }

    private void CreateBackupForTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        var terrainData = terrain.terrainData;
        _terrainHeightsBackup = 
            terrainData.GetHeights(0, 0, 
                terrainData.heightmapResolution, 
                terrainData.heightmapResolution);
        _terrainAlphasBackup =
            terrainData.GetAlphamaps(0, 0, 
                terrainData.heightmapResolution, 
                terrainData.heightmapResolution);
    }
    
    private void RestoreTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        terrainData.SetAlphamaps(1,1, _terrainAlphasBackup);
        terrainData.SetHeights(0, 0, _terrainHeightsBackup);
    }
}
