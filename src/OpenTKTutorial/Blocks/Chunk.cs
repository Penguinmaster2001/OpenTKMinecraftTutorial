
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using SimplexNoise;

using OpenTKTutorial.Graphics;



namespace OpenTKTutorial.Blocks;



internal class Chunk
{
    public const int ChunkSize = 16;



    private readonly List<Vector3> verts;
    private readonly List<Vector2> uvs;
    private readonly List<uint> indices;

    private uint indexCount;
    private VAO vao;
    private VBO<Vector3> vertexVbo;
    private VBO<Vector2> uvVbo;
    private IBO ibo;
    private Texture texture;

    private readonly Block[,,] blocks = new Block[ChunkSize, ChunkSize, ChunkSize];

    public Vector3i ChunkPosition;
    public Vector3i BlockOffset;


    /// <summary>
    /// Shorthand for chunk at origin for debugging
    /// </summary>
    public Chunk() : this(Vector3i.Zero) { }

    public Chunk(Vector3i chunkPosition)
    {
        verts = [];
        uvs = [];
        indices = [];

        indexCount = 0;

        ChunkPosition = chunkPosition;
        BlockOffset = ChunkSize * chunkPosition;
        float[,] heightMap = GenHeightMap();
        GenChunk(heightMap);
        BuildChunk();
    }



    public float[,] GenHeightMap()
    {
        float[,] heightMap = new float[ChunkSize, ChunkSize];

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                Vector3i blockPos = BlockOffset + new Vector3i(x, 0, z);
                heightMap[x, z] = Noise.CalcPixel2D(blockPos.X, blockPos.Z, 0.01f);
            }
        }

        return heightMap;
    }



    public void GenChunk(float[,] heightMap)
    {
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                int columnHeight = (int) heightMap[x, z] / 10;
                for (int y = 0; y < ChunkSize; y++)
                {
                    Vector3i chunkPos = new(x, y, z);
                    Vector3i blockPos = BlockOffset + chunkPos;

                    GenBlockType(blockPos, chunkPos, columnHeight);
                    GenFaces(blocks[x, y, z], chunkPos);
                }
            }
        }
    }



    private void GenBlockType(Vector3i blockPos, Vector3i chunkPos, int columnHeight)
    {
        Block newBlock = new(blockPos, BlockType.Empty);

        if (blockPos.Y < columnHeight - 2)
        {
            float caveNoise = (Noise.CalcPixel3D(blockPos.X, blockPos.Y, blockPos.Z, 0.01f) / 256.0f)
                            * (Noise.CalcPixel3D(blockPos.X + 1000, blockPos.Y + 1000, blockPos.Z + 1000, 0.05f) / 256.0f);

            if (0.1f < caveNoise && caveNoise < 0.2f)
            {
                newBlock = new(blockPos, BlockType.Empty);
            }
            else if (Noise.CalcPixel3D(blockPos.X, blockPos.Y, blockPos.Z, 0.05f) / 256.0f < 0.05f)
            {
                newBlock = new(blockPos, BlockType.Gem);
            }
            else
            {
                newBlock = new(blockPos, BlockType.Rock);
            }
        }
        else if (blockPos.Y < columnHeight)
        {
            newBlock = new(blockPos, BlockType.Dirt);
        }
        else if (blockPos.Y == columnHeight)
        {
            newBlock = new(blockPos, BlockType.Grass);
        }

        blocks[chunkPos.X, chunkPos.Y, chunkPos.Z] = newBlock;
    }



    public void GenFaces(Block block, Vector3i chunkPos)
    {
        int numFaces = 0;

        int x = chunkPos.X;
        int y = chunkPos.Y;
        int z = chunkPos.Z;

        // We must add faces to adjacent blocks
        if (block.Type == BlockType.Empty)
        {
            // Right face of block to left
            if (x > 0 && blocks[x - 1, y, z].Type != BlockType.Empty)
            {
                AddFace(blocks[x - 1, y, z], Faces.Right);
                numFaces++;
            }

            // Top face of block below
            if (y > 0 && blocks[x, y - 1, z].Type != BlockType.Empty)
            {
                AddFace(blocks[x, y - 1, z], Faces.Top);
                numFaces++;
            }

            // Front face of block behind
            if (z > 0 && blocks[x, y, z - 1].Type != BlockType.Empty)
            {
                AddFace(blocks[x, y, z - 1], Faces.Front);
                numFaces++;
            }
        }
        // Add faces to this block
        else
        {
            // Left face
            if (x == 0 || blocks[x - 1, y, z].Type == BlockType.Empty)
            {
                AddFace(block, Faces.Left);
                numFaces++;
            }

            // Bottom face
            if (y == 0 || blocks[x, y - 1, z].Type == BlockType.Empty)
            {
                AddFace(block, Faces.Bottom);
                numFaces++;
            }

            // Back face
            if (z == 0 || blocks[x, y, z - 1].Type == BlockType.Empty)
            {
                AddFace(block, Faces.Back);
                numFaces++;
            }

            // for blocks at edge of chunk
        
            // Right face
            if (x == ChunkSize - 1)
            {
                AddFace(block, Faces.Right);
                numFaces++;
            }

            // Top face
            if (y == ChunkSize - 1)
            {
                AddFace(block, Faces.Top);
                numFaces++;
            }

            // Front face
            if (z == ChunkSize - 1)
            {
                AddFace(block, Faces.Front);
                numFaces++;
            }
        }

        AddIndices(numFaces);
    }


    
    public void AddFace(Block block, Faces face)
    {
        FaceData faceData = block.GetFace(face);
        verts.AddRange(faceData.Vertices);
        uvs.AddRange(faceData.UV);
    }



    public void AddIndices(int amtFaces)
    {
        for (uint face = 0; face < amtFaces; face++)
        {
            indices.Add(0 + indexCount);
            indices.Add(1 + indexCount);
            indices.Add(2 + indexCount);
            indices.Add(2 + indexCount);
            indices.Add(3 + indexCount);
            indices.Add(0 + indexCount);

            indexCount += 4;
        }
    }



    public void BuildChunk()
    {
        vao = new();
        vao.Bind();

        vertexVbo = new(verts);
        vertexVbo.Bind();
        vao.LinkToVAO(0, 3, vertexVbo);
        vertexVbo.UnBind();

        uvVbo = new(uvs);
        uvVbo.Bind();
        vao.LinkToVAO(1, 2, uvVbo);
        uvVbo.UnBind();

        vao.UnBind();

        ibo = new(indices);

        texture = new("/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Textures/Textures.png");
    }



    public void Render(ShaderProgram shaderProgram)
    {
        shaderProgram.Bind();
        vao.Bind();
        texture.Bind();
        ibo.Bind();
        
        GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
    }



    public void Delete()
    {
        texture.Delete();
        ibo.Delete();
        vertexVbo.Delete();
        uvVbo.Delete();
        vao.Delete();
    }
}
