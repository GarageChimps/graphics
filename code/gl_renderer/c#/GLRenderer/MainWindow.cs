using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Input;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using OpenTK.Graphics.OpenGL;

namespace PixelShader
{
  class MainWindow : GameWindow
  {
    public static int SceneWidth = 512;
    public static int SceneHeight = 512;
    public static float FrameRate = 60;


    private string PixelShaderFilePath = Path.Combine("Shaders", "PixelShader.glsl");
    private string _pixelShaderSource;
    private string VertexShaderFilePath = Path.Combine("Shaders", "VertexShader.glsl");
    private string _vertexShaderSource;

    private int _vertexShaderHandle;
    private int _pixelShaderHandle;
    private int _shaderProgramHandle;
    private int _vaoHandle;
    private int _vertexPositionBufferHandle;
    private int _indicesBufferHandle;

    private int _viewProjectionHandle;

    private readonly Vector3[] _vertexPositionData =
    {
      new Vector3(-1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f,  1.0f,  -1.0f),
      new Vector3(-1.0f,  1.0f,  -1.0f)
    };

    private readonly int[] _indicesData =
    {
      0, 1, 2, 2, 3, 0
    };

    //Column order
    private readonly float[] _viewProjectionMatrix =
    {
      0.2f, 0, 0, 0,
      0, 0.2f, 0, 0,
      0, 0, 0, 0,
      0, 0, 0, 1
    };
    
    public MainWindow(int width, int height)
      : base(width, height,
        new OpenTK.Graphics.GraphicsMode(), "GL Renderer", GameWindowFlags.Default,
        DisplayDevice.Default, 3, 0,
        OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible | OpenTK.Graphics.GraphicsContextFlags.Debug)
    {

    }

    protected override void OnLoad(EventArgs e)
    {
      VSync = VSyncMode.On;
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.Texture2D);
      GL.ClearColor(0, 0.5f, 0.5f, 1);

      LoadShaders();
      CreateShaders();
      CreateBuffers();
      CreateVertexArrays();

      Console.WriteLine(GL.GetString(StringName.Version));

    }

    private void LoadShaders()
    {
      _pixelShaderSource = File.ReadAllText(PixelShaderFilePath);
      _vertexShaderSource = File.ReadAllText(VertexShaderFilePath);
    }

    protected virtual void CreateShaders()
    {
      GL.UseProgram(0);
      _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
      _pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      GL.ShaderSource(_vertexShaderHandle, _vertexShaderSource);
      GL.ShaderSource(_pixelShaderHandle, _pixelShaderSource);

      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_pixelShaderHandle);

      Console.WriteLine(GL.GetShaderInfoLog(_vertexShaderHandle));
      Console.WriteLine(GL.GetShaderInfoLog(_pixelShaderHandle));

      // Create program
      _shaderProgramHandle = GL.CreateProgram();

      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _pixelShaderHandle);

      GL.LinkProgram(_shaderProgramHandle);
      GL.UseProgram(_shaderProgramHandle);

      _viewProjectionHandle = GL.GetUniformLocation(_shaderProgramHandle, "viewProjection");
    }

    private void CreateBuffers()
    {
      GL.GenBuffers(1, out _vertexPositionBufferHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBufferHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_vertexPositionData.Length * Vector3.SizeInBytes),
          _vertexPositionData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _indicesBufferHandle);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indicesBufferHandle);
      GL.BufferData(BufferTarget.ElementArrayBuffer,
          new IntPtr(sizeof(uint) * _indicesData.Length),
          _indicesData, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    private void CreateVertexArrays()
    {
      GL.GenVertexArrays(1, out _vaoHandle);
      GL.BindVertexArray(_vaoHandle);

      GL.EnableVertexAttribArray(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBufferHandle);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indicesBufferHandle);

      GL.BindVertexArray(0);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Viewport(0, 0, this.Width, this.Height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.UniformMatrix4(_viewProjectionHandle, 1, false, _viewProjectionMatrix);

      GL.BindVertexArray(_vaoHandle);
      GL.DrawElements(BeginMode.Triangles, _indicesData.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
      SwapBuffers();
    }



    [STAThread]
    public static void Main()
    {
      using (var window = new MainWindow(SceneWidth, SceneHeight))
      {
        window.Run(FrameRate);
      }
    }
  }
}
