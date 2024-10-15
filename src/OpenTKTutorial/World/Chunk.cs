
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using SimplexNoise;

using OpenTKTutorial.Graphics;



namespace OpenTKTutorial.World;



internal class Chunk
{
    public const int Size = 16;



    private readonly List<Vector3> verts;
    private readonly List<Vector2> uvs;
    private readonly List<uint> indices;

    private uint indexCount;
    private VAO vao;
    private VBO<Vector3> vertexVbo;
    private VBO<Vector2> uvVbo;
    private IBO ibo;
    private Texture texture;

    private readonly Block[,,] blocks = new Block[Size, Size, Size];

    public Vector3 Position;


    /// <summary>
    /// Shorthand for chunk at origin for debugging
    /// </summary>
    public Chunk() : this(Vector3.Zero) { }

    public Chunk(Vector3 position)
    {
        verts = [];
        uvs = [];
        indices = [];

        indexCount = 0;

        Position = position;
        float[,] heightMap = GenChunk();
        GenBlocks(heightMap);
        GenFaces();
        BuildChunk();
    }



    public float[,] GenChunk()
    {
        float[,] heightMap = new float[Size, Size];

        for (int x = 0; x < Size; x++)
        {
            for (int z = 0; z < Size; z++)
            {
                heightMap[x, z] = Noise.CalcPixel2D(x, z, 0.01f);
            }
        }

        return heightMap;
    }



    public void GenBlocks(float[,] heightMap)
    {
        for (int x = 0; x < Size; x++)
        {
            for (int z = 0; z < Size; z++)
            {
                int columnHeight = (int) heightMap[x, z] / 20;
                for (int y = 0; y < Size; y++)
                {
                    if (y < columnHeight - 2)
                    {
                        blocks[x, y, z] = new(x, y, z, BlockType.TheRock);
                    }
                    else if (y < columnHeight)
                    {
                        blocks[x, y, z] = new(x, y, z, BlockType.Donal);
                    }
                    else if (y == columnHeight)
                    {
                        blocks[x, y, z] = new(x, y, z, BlockType.JungKook);
                    }
                    else
                    {
                        blocks[x, y, z] = new(x, y, z, BlockType.Empty);
                    }
                }
            }
        }
    }



    public void GenFaces()
    {
        for (int x = 0; x < Size; x++)
        {
            for (int z = 0; z < Size; z++)
            {
                for (int y = 0; y < Size; y++)
                {
                    int numFaces = 0;

                    Block block = blocks[x, y, z];
                    if (block.Type == BlockType.Empty) continue;

                    // Left face
                    if (x == 0 || blocks[x - 1, y, z].Type == BlockType.Empty)
                    {
                        AddFace(block, Faces.Left);
                        numFaces++;
                    }

                    // Right face
                    if (x == Size - 1 || blocks[x + 1, y, z].Type == BlockType.Empty)
                    {
                        AddFace(block, Faces.Right);
                        numFaces++;
                    }

                    // Back face
                    if (z == 0 || blocks[x, y, z - 1].Type == BlockType.Empty)
                    {
                        AddFace(block, Faces.Back);
                        numFaces++;
                    }

                    // Front face
                    if (z == Size - 1 || blocks[x, y, z + 1].Type == BlockType.Empty)
                    {
                        AddFace(block, Faces.Front);
                        numFaces++;
                    }

                    // Bottom face
                    if (y == 0 || blocks[x, y - 1, z].Type == BlockType.Empty)
                    {
                        AddFace(block, Faces.Bottom);
                        numFaces++;
                    }

                    // Top face
                    if (y == Size - 1 || blocks[x, y + 1, z].Type == BlockType.Empty)
                    {
                        AddFace(block, Faces.Top);
                        numFaces++;
                    }

                    AddIndices(numFaces);
                }
            }
        }
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
