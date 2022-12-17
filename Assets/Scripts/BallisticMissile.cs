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

        int craterCenterX;
        int craterCenterZ;
        
        // Calculate the craterWidth, craterCenterX and the widthOffset taking into account the edges of the terrain.
        int craterWidth = craterRadius * 2;
        int widthOffset = 0;
        if (xPosTerrainHeights - craterRadius <= 0)
        {
            widthOffset = xPosTerrainHeights - craterRadius;
            craterCenterX = craterWidth/2 + widthOffset;
        }
        else if (xPosTerrainHeights + craterRadius > terrainWidth)
        {
            widthOffset = -(craterRadius - (terrainWidth - xPosTerrainHeights));
            craterCenterX = craterRadius;
        }
        else
        {
            craterCenterX = craterWidth/2 + widthOffset;
        }
        craterWidth += widthOffset;
        
        // Calculate the craterHeight, craterCenterZ and the heightOffset taking into account the edges of the terrain.
        int craterHeight = craterRadius * 2;
        int heightOffset = 0;
        if (zPosTerrainHeights - craterRadius <= 0)
        {
            heightOffset = zPosTerrainHeights - craterRadius;
            craterCenterZ = craterHeight/2 + heightOffset;
        }
        else if (zPosTerrainHeights + craterRadius > terrainHeight)
        {
            heightOffset = -(craterRadius - (terrainHeight - zPosTerrainHeights));
            craterCenterZ = craterRadius;
        }
        else
        {
            craterCenterZ = craterHeight/2 + heightOffset;
        }
        craterHeight += heightOffset;
        
        float[,] modifiedHeights = 
            terrainData.GetHeights(xBase, zBase, craterWidth, craterHeight);
        
        Vector2 craterCenterIndex = new Vector2(x: craterCenterX, craterCenterZ);

        for (int x = 0; x < craterWidth; x++)
        {
            for (int z = 0; z < craterHeight; z++)
            {
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

