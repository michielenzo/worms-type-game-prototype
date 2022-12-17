using Unity.Netcode;
using UnityEngine;

public class BallisticMissile : NetworkBehaviour
{
    [SerializeField] 
    private GameObject explosionPrefab;

    // This makes sure there are no double explosions.
    private bool _exploded;

    [SerializeField]
    private float craterDepth = 0.02f;
    [SerializeField] 
    private int craterRadius = 3;

    private void OnCollisionEnter(Collision other)
    {
        if (IsServer && !_exploded)
        {
            _exploded = true;
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            explosion.GetComponent<NetworkObject>().Spawn();

            if (other.collider is TerrainCollider)
            {
                Vector3 collisionPoint = other.GetContact(0).point;
                TerraformCrater(collisionPoint);
                SynchronizeTerrainClientRpc(collisionPoint);
            }
            
            GetComponent<NetworkObject>().Despawn();
        }
    }
    
    private void TerraformCrater(Vector3 worldPoint)
    {
        Terrain terrain = Terrain.activeTerrain;

        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapResolution;
        int terrainHeight = terrainData.heightmapResolution;
        
        Vector3 terrainPosition = terrain.gameObject.transform.position;
        int xPosTerrainHeights = (int) ((worldPoint.x - terrainPosition.x)/ terrainData.size.x * terrainWidth);
        int zPosTerrainHeights = (int) ((worldPoint.z - terrainPosition.z)/ terrainData.size.z * terrainHeight);

        int xBase = xPosTerrainHeights - craterRadius >= 0 ? xPosTerrainHeights - craterRadius : 0;
        int zBase = zPosTerrainHeights - craterRadius >= 0 ? zPosTerrainHeights - craterRadius : 0;
        
        int craterWidth = craterRadius * 2;
        int widthOffset = 0;
        if (xPosTerrainHeights - craterRadius <= 0) widthOffset = xPosTerrainHeights - craterRadius;
        craterWidth += widthOffset;
        
        int craterHeight = craterRadius * 2;
        int heightOffset = 0;
        if (zPosTerrainHeights - craterRadius <= 0) heightOffset = zPosTerrainHeights - craterRadius;
        craterHeight += heightOffset;
        
        float[,] modifiedHeights = 
            terrainData.GetHeights(xBase, zBase, craterWidth, craterHeight);
        
        Debug.Log("xPosTerrainHeights: " + xPosTerrainHeights);
        Debug.Log("zPosTerrainHeights: " + zPosTerrainHeights);
        Debug.Log("xBase: " + xBase);
        Debug.Log("zBase: " + zBase);

        // ReSharper disable twice PossibleLossOfFraction
        Vector2 craterCenterIndex = new Vector2(x: craterWidth/2 + widthOffset, craterHeight/2 + heightOffset);
        
        Debug.Log("craterWidth: " + craterWidth + "craterHeight: " + craterHeight + " lengthDim 1: " + modifiedHeights.GetLength(0) + " lengthDim2: " + modifiedHeights.GetLength(1));

        for (int x = 0; x < craterWidth; x++)
        {
            //Debug.Log(x);
            for (int z = 0; z < craterHeight; z++)
            {
                //Debug.Log(x+ " "+z);
                float distanceFromCenter = Vector2.Distance(craterCenterIndex, new Vector2(x,z));

                if (distanceFromCenter <= craterRadius)
                {
                    double depthChange = craterDepth - distanceFromCenter / craterRadius * craterDepth;
                    modifiedHeights[z, x] -= (float) depthChange;
                }
            }
        }

        terrainData.SetHeights(xBase, zBase, modifiedHeights);
    }

    [ClientRpc]
    private void SynchronizeTerrainClientRpc(Vector3 worldPoint)
    {
        if(IsClient) TerraformCrater(worldPoint);
    }
}

