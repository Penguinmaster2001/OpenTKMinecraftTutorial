
using OpenTK.Mathematics;



namespace OpenTKTutorial.World;



internal static class TextureData
{
    private const int BlocksSize = 8;
    private const float textureAtlasSize = BlocksSize;

    public static readonly Dictionary<BlockType, Dictionary<Faces, List<Vector2>>> BlockTypeUVs = new() {
        { BlockType.Empty, new() {
                { Faces.Front , BlockSlotUV(0, 0) },
                { Faces.Back  , BlockSlotUV(0, 0) },
                { Faces.Left  , BlockSlotUV(0, 0) },
                { Faces.Right , BlockSlotUV(0, 0) },
                { Faces.Top   , BlockSlotUV(0, 0) },
                { Faces.Bottom, BlockSlotUV(0, 0) }
            }
        },
        { BlockType.Grass, new() {
                { Faces.Front , BlockSlotUV(2, 0) },
                { Faces.Back  , BlockSlotUV(2, 0) },
                { Faces.Left  , BlockSlotUV(2, 0) },
                { Faces.Right , BlockSlotUV(2, 0) },
                { Faces.Top   , BlockSlotUV(1, 0) },
                { Faces.Bottom, BlockSlotUV(2, 0) }
            }
        },
        { BlockType.Dirt, new() {
                { Faces.Front , BlockSlotUV(3, 0) },
                { Faces.Back  , BlockSlotUV(3, 0) },
                { Faces.Left  , BlockSlotUV(3, 0) },
                { Faces.Right , BlockSlotUV(3, 0) },
                { Faces.Top   , BlockSlotUV(3, 0) },
                { Faces.Bottom, BlockSlotUV(3, 0) }
            }
        },
        { BlockType.Rock, new() {
                { Faces.Front , BlockSlotUV(4, 0) },
                { Faces.Back  , BlockSlotUV(4, 0) },
                { Faces.Left  , BlockSlotUV(4, 0) },
                { Faces.Right , BlockSlotUV(4, 0) },
                { Faces.Top   , BlockSlotUV(4, 0) },
                { Faces.Bottom, BlockSlotUV(4, 0) }
            }
        },
        { BlockType.Gem, new() {
                { Faces.Front , BlockSlotUV(5, 0) },
                { Faces.Back  , BlockSlotUV(5, 0) },
                { Faces.Left  , BlockSlotUV(5, 0) },
                { Faces.Right , BlockSlotUV(5, 0) },
                { Faces.Top   , BlockSlotUV(5, 0) },
                { Faces.Bottom, BlockSlotUV(5, 0) }
            }
        }
    };



    private static List<Vector2> BlockSlotUV(int x, int y)
    {
        x %= BlocksSize;
        y = BlocksSize - 1 - (y % BlocksSize);

        return [
            new((x + 0) / textureAtlasSize, (y + 1) / textureAtlasSize),
            new((x + 1) / textureAtlasSize, (y + 1) / textureAtlasSize),
            new((x + 1) / textureAtlasSize, (y + 0) / textureAtlasSize),
            new((x + 0) / textureAtlasSize, (y + 0) / textureAtlasSize)
        ];
    }
}
