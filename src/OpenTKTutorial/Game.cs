
using StbImageSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;



namespace OpenTKTutorial;



internal class Game : GameWindow
{
    private int windowWidth;
    private int windowHeight;

    private float[] verts =
    {
         0.5f,  0.5f,  0.0f, // Top Right
        -0.5f,  0.5f,  0.0f, // Top Left
        -0.5f, -0.5f,  0.0f, // Bottom Left
         0.5f, -0.5f,  0.0f  // Bottom Right
    };

    private float[] texCoords =
    {
        1.0f, 1.0f,
        0.0f, 1.0f,
        0.0f, 0.0f,
        1.0f, 0.0f
    };

    private uint[] indices =
    {
        3, 0, 1,
        3, 1, 2
    };


    // Render pipeline vars
    private int vao;
    private int vbo;
    private int ibo;
    private int textureID;
    private int textureVbo;

    private int shaderProgram;



    public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        windowWidth = width;
        windowHeight = height;

        CenterWindow(new Vector2i(windowWidth, windowHeight));
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
        GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);

        // Put Vertex VBO into slot 0 of VAO
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(vao, 0);

        // Unbind Vertex VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


        // Texture VBO
        textureVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, textureVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);

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
        GL.ShaderSource(vertexShader, LoadShaderSource("/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Shaders/default.vert"));
        GL.CompileShader(vertexShader);

        int fragmentShader  = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, LoadShaderSource("/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Shaders/default.frag"));
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
        ImageResult theRock = ImageResult.FromStream(File.OpenRead("/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Textures/TheRock.png"),
                                                     ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, theRock.Width, theRock.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, theRock.Data);
        GL.BindTexture(TextureTarget.Texture2D, 0);
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
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(shaderProgram);

        GL.BindTexture(TextureTarget.Texture2D, textureID);

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 4);

        Context.SwapBuffers();
    }



    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
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



    private static int CreateShaderProgram(string vertexPath, string fragmentPath)
    {
        int vertexShader = CompileShader(ShaderType.VertexShader, File.ReadAllText(vertexPath));
        int fragmentShader = CompileShader(ShaderType.FragmentShader, File.ReadAllText(fragmentPath));

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Program linking failed: {infoLog}");
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return program;
    }



    private static int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            GL.DeleteShader(shader);
            throw new Exception($"{type} compilation failed: {infoLog}");
        }

        return shader;
    }



    private static void CheckGLError(string location)
    {
        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            throw new Exception($"OpenGL error at {location}: {error}");
        }
    }
}
