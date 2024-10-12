
using OpenTK.Graphics.OpenGL;
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
         0.0f,  0.5f,  0.0f,
        -0.5f, -0.5f,  0.0f,
         0.5f, -0.5f,  0.0f
    };


    // Render pipeline vars
    private int vao;
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

        vao = GL.GenVertexArray();

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);

        GL.BindVertexArray(vao);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(vao, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

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
    }



    protected override void OnUnload()
    {
        base.OnUnload();

        GL.DeleteVertexArray(vao);
        GL.DeleteProgram(shaderProgram);
    }



    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.ClearColor(0.2f, 0.3f, 0.8f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);


        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


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



    private void CheckGLError(string location)
    {
        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL error at {location}: {error}");
        }
    }
}
