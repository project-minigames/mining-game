using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Environment : MonoBehaviour
{
  public GameObject cameraObject;
  public GameObject chunkObject;
  public GameObject stoneVoxel;
  private Dictionary<ChunkPos, Chunk> loadedChunks;

  void Start()
  {
    loadedChunks = new Dictionary<ChunkPos, Chunk>();
  }

  void Update()
  {
      UpdateChunks();
  }

  private void UpdateChunks() {
    var cameraPos = cameraObject.transform.position;
    var chunkPos = new ChunkPos { x = ((int) cameraPos.x) >> 4, z = ((int) cameraPos.z) >> 4 };

    if (loadedChunks.Count > 0) {
      return; // todo; debug only
    }

    /* check if the chunk is loaded, otherwise generate it. */
    if (loadedChunks.ContainsKey(chunkPos)) {
      return;
    }

    /* create a fresh chunk */
    var newChunk = this.AddComponent<Chunk>();
    newChunk.Init(this, chunkPos);
    newChunk.CreateVoxels();
    loadedChunks.Add(chunkPos, newChunk);
  }
}

