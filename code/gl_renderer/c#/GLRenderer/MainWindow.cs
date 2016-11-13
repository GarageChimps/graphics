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

    //In OpenGL the way to identify resources in the GPU is with integer ids, which
    //are called "handles" in this code
    private int _vertexShaderHandle;
    private int _pixelShaderHandle;
    private int _shaderProgramHandle;
    private int _objectHandle;
    private int _vertexPositionBufferHandle;
    private int _vertexNormalBufferHandle;
    private int _facesBufferHandle;
    private int _transformationMatrixHandle;
    private int _lightPositionHandle;
    private int _lightColorHandle;
    private int _cameraPositionHandle;
    private int _materialColorHandle;

    // TODO: Replace this with vertices position data loaded from the mesh in the scene
    private readonly Vector3[] _vertexPositionData =
    {
      new Vector3(-1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f,  1.0f,  -1.0f),
      new Vector3(-1.0f,  1.0f,  -1.0f)
    };

    // TODO: Replace this with vertices normal data loaded from the mesh in the scene
    private readonly Vector3[] _vertexNormalData =
    {
      new Vector3(1.0f, 0.0f,  0.0f),
      new Vector3(0.0f, 1.0f,  0.0f),
      new Vector3(0.0f, 0.0f,  1.0f),
      new Vector3(1.0f, 0.0f,  1.0f)
    };

    //TODO: Replace this with face data loaded from the mesh in the scene
    private readonly uint[] _facesData =
    {
      0, 1, 2, //face 1
      2, 3, 0  //face 2
    };

    //TODO: Replace this with the orthographic projection transformation matrix
    //without including the image transformation (P * C)
    //Column order
    private readonly float[] _transformationMatrix =
    {
      0.2f, 0, 0, 0,
      0, 0.2f, 0, 0,
      0, 0, 0, 0,
      0, 0, 0, 1
    };

    //TODO: Replace this with scene parameters
    private readonly float[] _cameraPosition = {1, 0, 0};
    private readonly float[] _lightPosition = {0, 1, 0};
    private readonly float[] _lightColor = {0, 0, 1};
    private readonly float[] _materialColor = {1, 0, 0};
    
    public MainWindow(int width, int height)
      : base(width, height,
        new OpenTK.Graphics.GraphicsMode(), "GL Renderer", GameWindowFlags.Default,
        DisplayDevice.Default, 3, 0,
        OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible | OpenTK.Graphics.GraphicsContextFlags.Debug)
    {

    }

    // The initialization method in an OpenGL program is called once.
    // In the initialization method, we load and send the shaders to the GPU,
    // and also send all the geometry information
    protected override void OnLoad(EventArgs e)
    {
      VSync = VSyncMode.On;
      GL.Enable(EnableCap.DepthTest);
      GL.ClearColor(1, 1, 1, 1);

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

    // Shader source code is stored in GPU but we need to pass it to the GPU and instruct
    // it to compile the code and create the program that represents the rendering algorithm
    // that will be used
    protected virtual void CreateShaders()
    {
      _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
      _pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      GL.ShaderSource(_vertexShaderHandle, _vertexShaderSource);
      GL.ShaderSource(_pixelShaderHandle, _pixelShaderSource);

      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_pixelShaderHandle);

      Console.WriteLine(GL.GetShaderInfoLog(_vertexShaderHandle));
      Console.WriteLine(GL.GetShaderInfoLog(_pixelShaderHandle));
      
      _shaderProgramHandle = GL.CreateProgram();

      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _pixelShaderHandle);

      GL.LinkProgram(_shaderProgramHandle);
      
      _transformationMatrixHandle = GL.GetUniformLocation(_shaderProgramHandle, "transformationMatrix");
      _lightPositionHandle = GL.GetUniformLocation(_shaderProgramHandle, "lightPosition");
      _lightColorHandle = GL.GetUniformLocation(_shaderProgramHandle, "lightColor");
      _cameraPositionHandle = GL.GetUniformLocation(_shaderProgramHandle, "cameraPosition");
      _materialColorHandle = GL.GetUniformLocation(_shaderProgramHandle, "materialColor");
    }

    // Create the buffers to store the geometry information
    private void CreateBuffers()
    {
      GL.GenBuffers(1, out _vertexPositionBufferHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBufferHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_vertexPositionData.Length * Vector3.SizeInBytes),
          _vertexPositionData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _vertexNormalBufferHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexNormalBufferHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_vertexNormalData.Length * Vector3.SizeInBytes),
          _vertexNormalData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _facesBufferHandle);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _facesBufferHandle);
      GL.BufferData(BufferTarget.ElementArrayBuffer,
          new IntPtr(sizeof(uint) * _facesData.Length),
          _facesData, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    //OpenGL has the concept of "Vertex Array Objects (VAO)" which are groups of geometries,
    //shaders and textures that are associated to one object that wants to be drawn
    //The idea of a VAO is to associate to one single handle a group of configurations
    //that tell the GPU which geometry we want to render, with what shader and what textures
    private void CreateVertexArrays()
    {
      GL.GenVertexArrays(1, out _objectHandle);
      GL.BindVertexArray(_objectHandle);

      GL.EnableVertexAttribArray(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexPositionBufferHandle);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");

      GL.EnableVertexAttribArray(1);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexNormalBufferHandle);
      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
      GL.BindAttribLocation(_shaderProgramHandle, 1, "inNormal");

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _facesBufferHandle);

      GL.BindVertexArray(0);
    }

    // The render method of an OpenGL program will be called once per frame
    // erasing the previous image and generating a new one
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Viewport(0, 0, this.Width, this.Height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      // We need to indicate which shader to use before the drawing, and before sending
      // uniform data
      GL.UseProgram(_shaderProgramHandle);
      GL.UniformMatrix4(_transformationMatrixHandle, 1, false, _transformationMatrix);
      GL.Uniform3(_lightPositionHandle, 1, _lightPosition);
      GL.Uniform3(_lightColorHandle, 1, _lightColor);
      GL.Uniform3(_cameraPositionHandle, 1, _cameraPosition);
      GL.Uniform3(_materialColorHandle, 1, _materialColor);

      // We bind to our object handle so the GPU knows which geometries to draw
      GL.BindVertexArray(_objectHandle);

      // Draw elements tells the GPU to draw what type of geometry with the faces/vertex data
      // already stored in the GPU buffers
      GL.DrawElements(BeginMode.Triangles, _facesData.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

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
