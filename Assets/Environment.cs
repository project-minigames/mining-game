using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
  public GameObject player;
  public GameObject chunkPrefab;
  private Dictionary<ChunkPos, Chunk> loadedChunks;

  void Start()
  {
    Application.targetFrameRate = 60;

    loadedChunks = new Dictionary<ChunkPos, Chunk>();

    var spawnPos = createSpawnPosition();
    Debug.Log("Spawning the player at " + spawnPos);

    player.transform.position = spawnPos;
  }

  void Update()
  {
    UpdateChunks();
  }

  Chunk getLoadedChunk(ChunkPos pos)
  {
    return loadedChunks[pos];
  }

  Voxel getBlock(int x, int y, int z)
  {
    var chunkPos = new ChunkPos { x = x / Chunk.CHUNK_SIZE, z = z / Chunk.CHUNK_SIZE };

    if (!loadedChunks.ContainsKey(chunkPos))
    {
      return Voxel.EMPTY;
    }

    return loadedChunks[chunkPos].getVoxelRelative(x, y, z);
  }

  private Chunk createChunk(ChunkPos pos) {
    var chunkGameObject = Instantiate(chunkPrefab);
    var chunk = chunkGameObject.AddComponent<Chunk>();
    chunk.Init(this, pos);
  
    var position = new Vector3
    {
      x = pos.x * (Chunk.CHUNK_SIZE / 2),
      y = 0F,
      z = pos.z * (Chunk.CHUNK_SIZE / 2)
    };

    chunkGameObject.transform.position = position;
    return chunk;
  }

  private Vector3 createSpawnPosition()
  {
    var pos = new ChunkPos { x = 0, z = 0 };
    var chunk = createChunk(pos);
    loadedChunks.Add(pos, chunk);

    /* find a safe spawn position */
    var playerHeight = (int)player.transform.localScale.y;
    var spawnPos = new Vector3(Chunk.CHUNK_SIZE / 2, Chunk.CHUNK_HEIGHT + 1, Chunk.CHUNK_SIZE / 2);

    /*
    for (int y = 0; y < Chunk.CHUNK_HEIGHT; y += playerHeight) {
      for (int yy = 0; yy < playerHeight; yy++) {
        if (getBlock((int) spawnPos.x, y + yy, (int) spawnPos.x).type != VoxelType.AIR) {
          continue;
        }
      }

      spawnPos.y = y;
      break;
    }
     */

    return spawnPos;
  }

  private void UpdateChunks()
  {
    var playerPos = player.transform.position;
    var playerChunkPos = new ChunkPos
    {
      x = (int) playerPos.x / 32,
      z = (int) playerPos.z / 32
    };

    /* check if the chunk is loaded, otherwise generate it. */
    if (loadedChunks.ContainsKey(playerChunkPos))
    {
      return;
    }

    /* create a fresh chunk */
    var chunk = createChunk(playerChunkPos);
    loadedChunks.Add(playerChunkPos, chunk);
  }
}

