
using OpenTK.Mathematics;



namespace OpenTKTutorial.Blocks;



internal enum BlockType
{
    Empty,
    Grass,
    Dirt,
    Rock,
    Gem
}



internal enum Faces
{
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom
}



internal struct FaceData(List<Vector3> vertices, List<Vector2> uv)
{
    public List<Vector3> Vertices = vertices;
    public List<Vector2> UV = uv;
}



internal readonly struct RawFaceData
{
    public static readonly Dictionary<Faces, List<Vector3>> rawVertexData = new()
    {
        {Faces.Front, [
                new(-0.5f,  0.5f,  0.5f), // topleft vert
                new( 0.5f,  0.5f,  0.5f), // topright vert
                new( 0.5f, -0.5f,  0.5f), // bottomright vert
                new(-0.5f, -0.5f,  0.5f)  // bottomleft vert
            ]
        },
        {Faces.Back, [
                new(0.5f, 0.5f, -0.5f), // topleft vert
                new(-0.5f, 0.5f, -0.5f), // topright vert
                new(-0.5f, -0.5f, -0.5f), // bottomright vert
                new(0.5f, -0.5f, -0.5f), // bottomleft vert
            ]
        },
        {Faces.Left, [
                new(-0.5f, 0.5f, -0.5f), // topleft vert
                new(-0.5f, 0.5f, 0.5f), // topright vert
                new(-0.5f, -0.5f, 0.5f), // bottomright vert
                new(-0.5f, -0.5f, -0.5f), // bottomleft vert
            ]
        },
        {Faces.Right, [
                new(0.5f, 0.5f, 0.5f), // topleft vert
                new(0.5f, 0.5f, -0.5f), // topright vert
                new(0.5f, -0.5f, -0.5f), // bottomright vert
                new(0.5f, -0.5f, 0.5f), // bottomleft vert
            ]
        },
        {Faces.Top, [
                new(-0.5f, 0.5f, -0.5f), // topleft vert
                new(0.5f, 0.5f, -0.5f), // topright vert
                new(0.5f, 0.5f, 0.5f), // bottomright vert
                new(-0.5f, 0.5f, 0.5f), // bottomleft vert
            ]
        },
        {Faces.Bottom, [
                new(-0.5f, -0.5f, 0.5f), // topleft vert
                new(0.5f, -0.5f, 0.5f), // topright vert
                new(0.5f, -0.5f, -0.5f), // bottomright vert
                new(-0.5f, -0.5f, -0.5f), // bottomleft vert
            ]
        }
    };
}
