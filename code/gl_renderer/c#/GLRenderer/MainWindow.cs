using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Input;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace PixelShader
{
  class Object
  {
    public Vector3[] VertexPositionData { get; }
    public Vector3[] VertexNormalData { get; }
    public uint[] FacesData { get; }
    public float[] TransformationMatrix { get; }
    public float[] MaterialColor { get; }

    public int VertexPositionBufferHandle { get; set; }
    public int VertexNormalBufferHandle { get; set; }
    public int FacesBufferHandle { get; set; }
    public int ObjectHandle { get; set; }
    public int TextureId { get; set; }

    public Object(Vector3[] vertexPositionData, Vector3[] vertexNormalData, uint[] facesData, float[] transformationMatrix, float[] materialColor)
    {
      VertexPositionData = vertexPositionData;
      VertexNormalData = vertexNormalData;
      FacesData = facesData;
      TransformationMatrix = transformationMatrix;
      MaterialColor = materialColor;
    }
  }

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
    private int _transformationMatrixHandle;
    private int _lightPositionHandle;
    private int _lightColorHandle;
    private int _cameraPositionHandle;
    private int _materialColorHandle;
    private int _textureHandle;


    //TODO: Replace this with scene parameters
    private readonly float[] _cameraPosition = { 1, 0, 0 };
    private readonly float[] _lightPosition = { 0, 1, 0 };
    private readonly float[] _lightColor = { 0, 0, 1 };

    private readonly List<Object> _objects = new List<Object>();

    public MainWindow(int width, int height)
      : base(width, height,
        new OpenTK.Graphics.GraphicsMode(), "GL Renderer", GameWindowFlags.Default,
        DisplayDevice.Default, 3, 0,
        OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible | OpenTK.Graphics.GraphicsContextFlags.Debug)
    {
      var obj1 = new Object(
        new[] {
          new Vector3(-1.0f, -1.0f,  0.9f),
          new Vector3( 1.0f, -1.0f,  0.9f),
          new Vector3( 1.0f,  1.0f,  0.9f),
          new Vector3(-1.0f,  1.0f,  0.9f)
        },
        new[] {
          new Vector3(1.0f, 0.0f,  0.0f),
          new Vector3(0.0f, 1.0f,  0.0f),
          new Vector3(1.0f, 0.0f,  1.0f),
          new Vector3(1.0f, 1.0f,  0.0f)
        },
        new uint[]{
          0, 1, 2, //face 1
          2, 3, 0  //face 2
        },
        new[]
        {
           0.2f, 0, 0, 0,
          0, 0.2f, 0, 0,
          0, 0, 1, 0,
          0, 0, 0, 1
        },
        new float[] { 1, 0, 0 }
        );
      var obj2 = new Object(
        new[] {
          new Vector3(-1.0f, -1.0f,  -0.9f),
          new Vector3( 1.0f, -1.0f,  -0.9f),
          new Vector3( 1.0f,  1.0f,  -0.9f),
          new Vector3(-1.0f,  1.0f,  -0.9f)
        },
        new[] {
          new Vector3(0.0f, 0.0f,  1.0f),
          new Vector3(0.0f, 0.0f,  1.0f),
          new Vector3(0.0f, 0.0f,  1.0f),
          new Vector3(0.0f, 0.0f,  1.0f)
        },
        new uint[]{
          0, 1, 2, //face 1
          2, 3, 0  //face 2
        },
        new[]
        {
           0.2f, 0, 0, 0,
          0, 0.2f, 0, 0,
          0, 0, 1, 0,
          0.3f, 0.3f, 0, 1
        },
        new float[] { 0, 0, 1 }
        );
      _objects.Add(obj1);
      _objects.Add(obj2);
    }

    // The initialization method in an OpenGL program is called once.
    // In the initialization method, we load and send the shaders to the GPU,
    // and also send all the geometry information
    protected override void OnLoad(EventArgs e)
    {
      VSync = VSyncMode.On;
      GL.Enable(EnableCap.DepthTest);
// GL.DepthRange(1, 0);
      GL.ClearColor(1, 1, 1, 1);

      LoadShaders();
      CreateShaders();
      CreateBuffers();
      CreateVertexArrays();
      //We create  texture from the example image file and assign it to the first object
      int textureId = CreateTexture(Bitmap.FromFile("chess.png") as Bitmap, TextureUnit.Texture0, TextureMinFilter.Nearest,
        TextureMagFilter.Linear);
      _objects[0].TextureId = textureId;
      //We wont assign a texture for the second object in this example, the default value of textureId 0
      //is ineterpreted by the GPU as "no texture"

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
      _textureHandle = GL.GetUniformLocation(_shaderProgramHandle, "textureSampler");
    }

    // Create the buffers to store the geometry information
    private void CreateBuffers()
    {
      foreach (var obj in _objects)
      {
        var vertexPositionBufferHandle = 0;
        GL.GenBuffers(1, out vertexPositionBufferHandle);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexPositionBufferHandle);
        GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(obj.VertexPositionData.Length*Vector3.SizeInBytes),
          obj.VertexPositionData, BufferUsageHint.StaticDraw);
        obj.VertexPositionBufferHandle = vertexPositionBufferHandle;

        var vertexNormalBufferHandle = 0;
        GL.GenBuffers(1, out vertexNormalBufferHandle);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexNormalBufferHandle);
        GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(obj.VertexNormalData.Length*Vector3.SizeInBytes),
          obj.VertexNormalData, BufferUsageHint.StaticDraw);
        obj.VertexNormalBufferHandle = vertexNormalBufferHandle;

        var facesBufferHandle = 0;
        GL.GenBuffers(1, out facesBufferHandle);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, facesBufferHandle);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
          new IntPtr(sizeof (uint)*obj.FacesData.Length),
          obj.FacesData, BufferUsageHint.StaticDraw);
        obj.FacesBufferHandle = facesBufferHandle;
      }

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    //OpenGL has the concept of "Vertex Array Objects (VAO)" which are groups of geometries,
    //shaders and textures that are associated to one object that wants to be drawn
    //The idea of a VAO is to associate to one single handle a group of configurations
    //that tell the GPU which geometry we want to render, with what shader and what textures
    private void CreateVertexArrays()
    {
      foreach (var obj in _objects)
      {
        var objectHandle = 0;
        GL.GenVertexArrays(1, out objectHandle);
        GL.BindVertexArray(objectHandle);

        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, obj.VertexPositionBufferHandle);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
        GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");

        GL.EnableVertexAttribArray(1);
        GL.BindBuffer(BufferTarget.ArrayBuffer, obj.VertexNormalBufferHandle);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
        GL.BindAttribLocation(_shaderProgramHandle, 1, "inColor");

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, obj.FacesBufferHandle);
        obj.ObjectHandle = objectHandle;
      }
      

      GL.BindVertexArray(0);
    }

    //Creates a texture in GPU memory, and copies the bitmap image into it. 
    //The GPU has N texture units available (usually 8), so we need to assign this texture to one of the units
    private int CreateTexture(Bitmap texture, TextureUnit unit, TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
      int textureId = GL.GenTexture();
      GL.ActiveTexture(unit);
      GL.BindTexture(TextureTarget.Texture2D, textureId);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0,
        PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

      var bmpData = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, texture.Width, texture.Height, PixelFormat.Bgra,
        PixelType.UnsignedByte, bmpData.Scan0);

      texture.UnlockBits(bmpData);

      GL.BindTexture(TextureTarget.Texture2D, 0);

      return textureId;
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
      GL.Uniform3(_lightPositionHandle, 1, _lightPosition);
      GL.Uniform3(_lightColorHandle, 1, _lightColor);
      GL.Uniform3(_cameraPositionHandle, 1, _cameraPosition);
      foreach (var obj in _objects)
      {
        GL.UniformMatrix4(_transformationMatrixHandle, 1, false, obj.TransformationMatrix);
        GL.Uniform3(_materialColorHandle, 1, obj.MaterialColor);

        //We indicate that we will use texture unit 0 for this drawing, and bind it to our previously
        //create texture
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, obj.TextureId);
        //We also need to tell the shader which that the sampler will use the texture stored in texture unit 0
        GL.Uniform1(_textureHandle, 0);

        // We bind to our object handle so the GPU knows which geometries to draw
        GL.BindVertexArray(obj.ObjectHandle);

        // Draw elements tells the GPU to draw what type of geometry with the faces/vertex data
        // already stored in the GPU buffers
        GL.DrawElements(BeginMode.Triangles, obj.FacesData.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
      }
      

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
