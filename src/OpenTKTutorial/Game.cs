
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKTutorial.Graphics;



namespace OpenTKTutorial;



internal class Game : GameWindow
{
    private Camera camera;

    private int windowWidth;
    private int windowHeight;

    private List<Vector3> verts = [
        // front face
        new Vector3(-0.5f, 0.5f, 0.5f), // topleft vert
        new Vector3(0.5f, 0.5f, 0.5f), // topright vert
        new Vector3(0.5f, -0.5f, 0.5f), // bottomright vert
        new Vector3(-0.5f, -0.5f, 0.5f), // bottomleft vert
        // right face
        new Vector3(0.5f, 0.5f, 0.5f), // topleft vert
        new Vector3(0.5f, 0.5f, -0.5f), // topright vert
        new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
        new Vector3(0.5f, -0.5f, 0.5f), // bottomleft vert
        // back face
        new Vector3(0.5f, 0.5f, -0.5f), // topleft vert
        new Vector3(-0.5f, 0.5f, -0.5f), // topright vert
        new Vector3(-0.5f, -0.5f, -0.5f), // bottomright vert
        new Vector3(0.5f, -0.5f, -0.5f), // bottomleft vert
        // left face
        new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
        new Vector3(-0.5f, 0.5f, 0.5f), // topright vert
        new Vector3(-0.5f, -0.5f, 0.5f), // bottomright vert
        new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
        // top face
        new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
        new Vector3(0.5f, 0.5f, -0.5f), // topright vert
        new Vector3(0.5f, 0.5f, 0.5f), // bottomright vert
        new Vector3(-0.5f, 0.5f, 0.5f), // bottomleft vert
        // bottom face
        new Vector3(-0.5f, -0.5f, 0.5f), // topleft vert
        new Vector3(0.5f, -0.5f, 0.5f), // topright vert
        new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
        new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
    ];

    private List<Vector2> texCoords = [
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),

        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),

        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),

        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),

        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),

        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),
    ];

    private List<uint> indices = [
        // first face
        // top triangle
        0, 1, 2,
        // bottom triangle
        2, 3, 0,

        4, 5, 6,
        6, 7, 4,

        8, 9, 10,
        10, 11, 8,

        12, 13, 14,
        14, 15, 12,

        16, 17, 18,
        18, 19, 16,

        20, 21, 22,
        22, 23, 20
    ];


    // Render pipeline vars
    private VAO vao;
    private IBO ibo;
    private Texture texture;

    private ShaderProgram shaderProgram;
    private string vertexShaderPath = "/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Shaders/default.vert";
    private string fragmentShaderPath = "/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Shaders/default.frag";
    private string texturePath = "/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Textures/TheRock.png";

    private float yRot = 0.0f;



    public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        windowWidth = width;
        windowHeight = height;

        CenterWindow(new Vector2i(windowWidth, windowHeight));

        camera = new(windowWidth, windowHeight, Vector3.Zero);

        vao = new();
        VBO<Vector3> vbo = new(verts);
        vao.LinkToVAO(0, 3, vbo);
        VBO<Vector2> texVbo = new(texCoords);
        vao.LinkToVAO(1, 2, texVbo);

        ibo = new(indices);

        shaderProgram = new(vertexShaderPath, fragmentShaderPath);

        texture = new(texturePath);
    }



    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        windowWidth = e.Width;
        windowHeight = e.Height;
        GL.Viewport(0, 0, windowWidth, windowHeight);
    }



    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);
    }



    protected override void OnUnload()
    {
        base.OnUnload();

        texture.Delete();
        ibo.Delete();
        vao.Delete();
        shaderProgram.Delete();
    }



    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.ClearColor(0.2f, 0.3f, 0.8f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shaderProgram.Bind();
        vao.Bind();
        ibo.Bind();
        texture.Bind();

        // Transformation matrices
        Matrix4 model = Matrix4.Identity;
        Matrix4 view = camera.ViewMatrix;
        Matrix4 projection = camera.ProjectionMatrix;

        model *= Matrix4.CreateRotationY(2.0f * yRot);
        model *= Matrix4.CreateRotationX(0.5f * yRot);
        model *= Matrix4.CreateTranslation(0.0f, 0.0f, -5.0f);

        int modelLocation = GL.GetUniformLocation(shaderProgram.ID, "model");
        int viewLocation = GL.GetUniformLocation(shaderProgram.ID, "view");
        int projectionLocation = GL.GetUniformLocation(shaderProgram.ID, "projection");

        GL.UniformMatrix4(modelLocation, true, ref model);
        GL.UniformMatrix4(viewLocation, true, ref view);
        GL.UniformMatrix4(projectionLocation, true, ref projection);

        GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 4);

        Context.SwapBuffers();
    }



    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        yRot += 2.0f * (float) args.Time;

        MouseState mouseState = MouseState;
        KeyboardState keyboardState = KeyboardState;

        if (keyboardState.IsKeyReleased(Keys.Escape))
        {
            CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
        }

        camera.Update(keyboardState, mouseState, args);
    }



    public static string LoadShaderSource(string filePath)
    {
        string shaderSource = "";

        try
        {
            using (StreamReader reader = new(filePath))
            {
                shaderSource = reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load shader!!\nFilepath: {filePath}\n{e.Message}");
        }

        return shaderSource;
    }



    // private static int CreateShaderProgram(string vertexPath, string fragmentPath)
    // {
    //     int vertexShader = CompileShader(ShaderType.VertexShader, File.ReadAllText(vertexPath));
    //     int fragmentShader = CompileShader(ShaderType.FragmentShader, File.ReadAllText(fragmentPath));

    //     int program = GL.CreateProgram();
    //     GL.AttachShader(program, vertexShader);
    //     GL.AttachShader(program, fragmentShader);
    //     GL.LinkProgram(program);

    //     GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
    //     if (success == 0)
    //     {
    //         string infoLog = GL.GetProgramInfoLog(program);
    //         throw new Exception($"Program linking failed: {infoLog}");
    //     }

    //     GL.DeleteShader(vertexShader);
    //     GL.DeleteShader(fragmentShader);

    //     return program;
    // }



    // private static int CompileShader(ShaderType type, string source)
    // {
    //     int shader = GL.CreateShader(type);
    //     GL.ShaderSource(shader, source);
    //     GL.CompileShader(shader);

    //     GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
    //     if (success == 0)
    //     {
    //         string infoLog = GL.GetShaderInfoLog(shader);
    //         GL.DeleteShader(shader);
    //         throw new Exception($"{type} compilation failed: {infoLog}");
    //     }

    //     return shader;
    // }



    // private static void CheckGLError(string location)
    // {
    //     ErrorCode error = GL.GetError();
    //     if (error != ErrorCode.NoError)
    //     {
    //         throw new Exception($"OpenGL error at {location}: {error}");
    //     }
    // }
}
