
using OpenTK.Mathematics;
using OpenTKTutorial.Graphics;

namespace OpenTKTutorial.Blocks;



internal class World
{
    private readonly Dictionary<Vector3i, Chunk> chunks;
    private int genRadius = 2;

    private readonly Queue<Vector3i> chunksToGen;
    private int maxGenPerUpdate = 8;



    public World()
    {
        chunks = [];
        chunksToGen = [];
    }



    public void Update(Vector3 cameraPos)
    {
        Vector3i cameraChunk = (Vector3i) (cameraPos / Chunk.ChunkSize);

        for (int x = -genRadius; x <= genRadius; x++)
        {
            for (int y = -genRadius; y <= genRadius; y++)
            {
                for (int z = -genRadius; z <= genRadius; z++)
                {
                    Vector3i checkChunkPos = cameraChunk + new Vector3i(x, y, z);

                    if (!chunks.ContainsKey(checkChunkPos))
                    {
                        chunksToGen.Enqueue(checkChunkPos);
                    }
                }
            }
        }

        GenEnqueuedChunks();
    }



    private void GenEnqueuedChunks()
    {
        int numGenerated = 0;

        while (numGenerated < maxGenPerUpdate && chunksToGen.TryDequeue(out Vector3i genPos))
        {
            if (!chunks.ContainsKey(genPos))
            {
                chunks[genPos] = new(genPos);
                numGenerated++;
            }
        }
    }



    public void Render(ShaderProgram shaderProgram)
    {
        foreach (Chunk chunk in chunks.Values)
        {
            chunk.Render(shaderProgram);
        }
    }



    public void Delete()
    {
        foreach (Chunk chunk in chunks.Values)
        {
            chunk.Delete();
        }
    }
}
