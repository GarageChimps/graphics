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
    private int _fragmentShaderHandle;
    private int _shaderProgramHandle;
    private int _vaoHandle;
    private int _positionVboHandle;
    private int _eboHandle;
    private int _viewProjectionHandler;

    private readonly Vector3[] _positionVboData =
    {
      new Vector3(-1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f,  1.0f,  -1.0f),
      new Vector3(-1.0f,  1.0f,  -1.0f)
    };

    private readonly int[] _indicesVboData =
    {
      0, 1, 2, 2, 3, 0
    };

    private readonly float[] _viewProjectionMatrix =
    {
      0.2f, 0, 0, 0, 0, 0.2f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1
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
      GL.ClearColor(Color.AliceBlue);

      LoadShaders();
      CreateShaders();
      CreateVBOs();
      CreateVAOs();

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
      _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      GL.ShaderSource(_vertexShaderHandle, _vertexShaderSource);
      GL.ShaderSource(_fragmentShaderHandle, _pixelShaderSource);

      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_fragmentShaderHandle);

      Console.WriteLine(GL.GetShaderInfoLog(_fragmentShaderHandle));

      // Create program
      _shaderProgramHandle = GL.CreateProgram();

      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _fragmentShaderHandle);

      GL.LinkProgram(_shaderProgramHandle);
      GL.UseProgram(_shaderProgramHandle);

      _viewProjectionHandler = GL.GetUniformLocation(_shaderProgramHandle, "viewProjection");
    }

    private void CreateVBOs()
    {
      GL.GenBuffers(1, out _positionVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_positionVboData.Length * Vector3.SizeInBytes),
          _positionVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _eboHandle);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);
      GL.BufferData(BufferTarget.ElementArrayBuffer,
          new IntPtr(sizeof(uint) * _indicesVboData.Length),
          _indicesVboData, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    private void CreateVAOs()
    {
      GL.GenVertexArrays(1, out _vaoHandle);
      GL.BindVertexArray(_vaoHandle);

      GL.EnableVertexAttribArray(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);

      GL.BindVertexArray(0);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Viewport(0, 0, this.Width, this.Height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.UniformMatrix4(_viewProjectionHandler, 1, true, _viewProjectionMatrix);

      GL.BindVertexArray(_vaoHandle);
      GL.DrawElements(BeginMode.LineStrip, _indicesVboData.Length,
          DrawElementsType.UnsignedInt, IntPtr.Zero);
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
