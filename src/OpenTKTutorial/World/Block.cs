
using OpenTK.Mathematics;



namespace OpenTKTutorial.World;



internal class Block
{
    public Vector3 Position;
    public BlockType Type;



    private readonly Dictionary<Faces, FaceData> faces;
    public Dictionary<Faces, List<Vector2>> blockUV;



    public Block(float x, float y, float z, BlockType blockType = BlockType.Empty) : this(new Vector3(x, y, z), blockType) { }

    public Block(Vector3 position, BlockType blockType = BlockType.Empty)
    {
        Position = position;
        Type = blockType;

        blockUV = TextureData.BlockTypeUVs[blockType];

        faces = new() {
            {Faces.Front , new(AddTransformedVertices(RawFaceData.rawVertexData[Faces.Front ]), blockUV[Faces.Front ])},
            {Faces.Back  , new(AddTransformedVertices(RawFaceData.rawVertexData[Faces.Back  ]), blockUV[Faces.Back  ])},
            {Faces.Left  , new(AddTransformedVertices(RawFaceData.rawVertexData[Faces.Left  ]), blockUV[Faces.Left  ])},
            {Faces.Right , new(AddTransformedVertices(RawFaceData.rawVertexData[Faces.Right ]), blockUV[Faces.Right ])},
            {Faces.Top   , new(AddTransformedVertices(RawFaceData.rawVertexData[Faces.Top   ]), blockUV[Faces.Top   ])},
            {Faces.Bottom, new(AddTransformedVertices(RawFaceData.rawVertexData[Faces.Bottom]), blockUV[Faces.Bottom])}
        };
    }



    public List<Vector3> AddTransformedVertices(List<Vector3> vertices) => vertices.Select(vert => vert + Position).ToList();



    public FaceData GetFace(Faces face) => faces[face];
}
