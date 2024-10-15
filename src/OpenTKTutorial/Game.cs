
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

using OpenTKTutorial.Graphics;
using OpenTKTutorial.World;



namespace OpenTKTutorial;



internal class Game : GameWindow
{
    private Camera camera;

    private Chunk? chunk;

    private int windowWidth;
    private int windowHeight;

    private ShaderProgram? shaderProgram;
    private string vertexShaderPath = "/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Shaders/default.vert";
    private string fragmentShaderPath = "/home/penguin/Documents/Projects/OpenTKTutorial/src/OpenTKTutorial/Shaders/default.frag";



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

        shaderProgram = new(vertexShaderPath, fragmentShaderPath);

        chunk = new();

        GL.Enable(EnableCap.DepthTest);
        GL.FrontFace(FrontFaceDirection.Cw);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
    }



    protected override void OnUnload()
    {
        base.OnUnload();

        chunk?.Delete();
        shaderProgram?.Delete();
    }



    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.ClearColor(0.2f, 0.3f, 0.8f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Transformation matrices
        Matrix4 model = Matrix4.Identity;
        Matrix4 view = camera.ViewMatrix;
        Matrix4 projection = camera.ProjectionMatrix;

        if (shaderProgram is not null)
        {
            int modelLocation = GL.GetUniformLocation(shaderProgram.ID, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram.ID, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.ID, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            chunk?.Render(shaderProgram);
        }
        else
        {
            throw new Exception($"shaderProgram is null!!");
        }


        Context.SwapBuffers();
    }



    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

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
