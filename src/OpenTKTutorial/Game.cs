
using StbImageSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;



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

    private uint[] indices = [
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
    private int vao;
    private int vbo;
    private int ibo;
    private int textureID;
    private int textureVbo;

    private int shaderProgram;
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

        // Create and bind VAO
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        // Vertex VBO
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, verts.Count * Vector3.SizeInBytes, verts.ToArray(), BufferUsageHint.StaticDraw);

        // Put Vertex VBO into slot 0 of VAO
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(vao, 0);

        // Unbind Vertex VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


        // Texture VBO
        textureVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, textureVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);

        // Put Texture VBO into slot 1 of VAO
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(vao, 1);

        // Unbind Texture VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        // Unbind VAO
        GL.BindVertexArray(0);

        ibo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        shaderProgram = GL.CreateProgram();

        int vertexShader  = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, LoadShaderSource(vertexShaderPath));
        GL.CompileShader(vertexShader);

        int fragmentShader  = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentShaderPath));
        GL.CompileShader(fragmentShader);

        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);

        GL.LinkProgram(shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        textureID = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, textureID);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult theRock = ImageResult.FromStream(File.OpenRead(texturePath),
                                                     ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, theRock.Width, theRock.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, theRock.Data);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.Enable(EnableCap.DepthTest);
    }



    protected override void OnUnload()
    {
        base.OnUnload();

        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ibo);
        GL.DeleteTexture(textureID);
        GL.DeleteProgram(shaderProgram);
    }



    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.ClearColor(0.2f, 0.3f, 0.8f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(shaderProgram);

        GL.BindTexture(TextureTarget.Texture2D, textureID);


        // Transformation matrices
        Matrix4 model = Matrix4.Identity;
        Matrix4 view = camera.ViewMatrix;
        Matrix4 projection = camera.ProjectionMatrix;

        model *= Matrix4.CreateRotationY(2.0f * yRot);
        model *= Matrix4.CreateRotationX(0.5f * yRot);
        model *= Matrix4.CreateTranslation(0.0f, 0.0f, -5.0f);

        int modelLocation = GL.GetUniformLocation(shaderProgram, "model");
        int viewLocation = GL.GetUniformLocation(shaderProgram, "view");
        int projectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

        GL.UniformMatrix4(modelLocation, true, ref model);
        GL.UniformMatrix4(viewLocation, true, ref view);
        GL.UniformMatrix4(projectionLocation, true, ref projection);

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
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
