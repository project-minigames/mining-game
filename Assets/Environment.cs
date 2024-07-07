using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
  private static readonly int VIEW_DISTANCE = 1;
  public GameObject player;
  public GameObject chunkPrefab;
  private Dictionary<ChunkPos, Chunk> loadedChunks;
  public int seed;

  void Start()
  {
    Application.targetFrameRate = 60;

    seed = Random.Range(0, 10_000);
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
      x = pos.x * Chunk.CHUNK_SIZE,
      y = 0F,
      z = pos.z * Chunk.CHUNK_SIZE
    };

//     Debug.Log("chunk pos: (" + pos.x + ", " + pos.z + ") - absolute pos: (" + position.x + ", " + position.z + ")");
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
      x = (int) playerPos.x >> 4,
      z = (int) playerPos.z >> 4
    };
//     Debug.Log("(" + playerChunkPos.x + ", " + playerChunkPos.z + ")");

    /* load chunks around the player's chunk */
    for (int xx = playerChunkPos.x - VIEW_DISTANCE; xx < playerChunkPos.x + VIEW_DISTANCE; xx++)
    {
      for (int zz = playerChunkPos.z - VIEW_DISTANCE; zz < playerChunkPos.z + VIEW_DISTANCE; zz++)
      {
        var chunkPos = new ChunkPos
        {
          x = xx,
          z = zz
        };

        if (loadedChunks.ContainsKey(chunkPos))
        {
          continue;
        }

        /* create a fresh chunk */
        var chunk = createChunk(chunkPos);
        loadedChunks.Add(chunk.pos, chunk);
      }
    }
  }
}

