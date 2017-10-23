using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace raytracer
{
  class Mesh : IObject
  {
    private readonly bool _computeVertexNormals;
    private Transform _transform;
    private Box _boundingBox;
    private OctTreeNode _octTree;

    public string FilePath { get; set; }
    public List<string> Materials { get; set; }
    
    public List<Vector> Positions { get; private set; } 
    public List<Vector> Normals { get; private set; } 
    public List<Vector> TexCoords { get; private set; } 
    public List<Face> Faces { get; private set; } 

    public Mesh(string filePath, List<string> materials, bool computeVertexNormals, Transform transform)
    {
      _computeVertexNormals = computeVertexNormals;
      _transform = transform;
      _boundingBox = new Box(new Vector(float.MaxValue, float.MaxValue, float.MaxValue), new Vector(float.MinValue, float.MinValue, float.MinValue));

      FilePath = filePath;
      Materials = materials;

      Positions = new List<Vector>();
      Normals = new List<Vector>();
      TexCoords = new List<Vector>();
      Faces = new List<Face>();
    }

    public void Init()
    {
      LoadFromObj(FilePath);
      _octTree = OctTreeNode.GenerateOctTree(_boundingBox, 0, 3);
      _octTree.Initialize(this);
      if (_computeVertexNormals)
        ComputeVertexNormals();
    }

    private void LoadFromObj(string filePath)
    {
      using (var reader = new StreamReader(filePath))
      {
        while (!reader.EndOfStream)
        {
          var line = reader.ReadLine();
          var parts = line.Split();
          if (parts.Length == 0)
            continue;

          if (parts[0] == "v")
          {
            var vp = new Vector(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            var tvp = _transform.TransformPosition(vp);
            Positions.Add(tvp);
            _boundingBox.Update(tvp);
          }
          else if (parts[0] == "vt")
            TexCoords.Add(new Vector(float.Parse(parts[1]), float.Parse(parts[2]), 0));
          else if (parts[0] == "vn")
          {
            var vn = new Vector(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            Normals.Add(_transform.TransformDirection(vn).Normalized);
          }
          else if (parts[0] == "f")
          {
            ParseFace(parts);
          }
        }
      }
    }

    private void ParseFace(string[] parts)
    {
      var positionIndices = new List<int>();
      var texCoordIndices = new List<int>();
      var normalIndices = new List<int>();
      for (int i = 1; i < 4; i++)
      {
        var indices = parts[i].Split(new[] { '/' }, StringSplitOptions.None);
        if (indices.Length > 0 && indices[0] != "")
          positionIndices.Add(int.Parse(indices[0]) - 1);
        if (indices.Length > 1 && indices[1] != "")
          texCoordIndices.Add(int.Parse(indices[1]) - 1);
        if (indices.Length > 2 && indices[2] != "")
          normalIndices.Add(int.Parse(indices[2]) - 1);
      }
      Faces.Add(new Face(positionIndices, texCoordIndices, normalIndices, this));
    }

    private void ComputeVertexNormals()
    {
      if (Normals.Count > 0)
        return;
      List<List<Face>> facesForVertices = GetFacesForVertices();
      foreach (var facesForVertex in facesForVertices)
      {
        Vector normal = facesForVertex.Select(f => f.FaceNormal).Aggregate((acc, v) => acc + v).Normalized;
        Normals.Add(normal.Normalized);
      }
      foreach (var face in Faces)
      {
        face.NormalIndices = face.PositionIndices.ToList();
      }
    }

    private List<List<Face>> GetFacesForVertices()
    {
      var facesForVertices = new List<List<Face>>();
      foreach (var position in Positions)
      {
        facesForVertices.Add(new List<Face>());
      }
      foreach (var face in Faces)
      {
        foreach (var positionIndex in face.PositionIndices)
        {
          facesForVertices[positionIndex].Add(face);
        }
      }

      return facesForVertices;
    }

    public Tuple<float, IObject> Intersect(Ray ray)
    {
      return _octTree.Intersect(ray);
    }

    public Vector GetNormal(Vector p, float time)
    {
      return new Vector();
    }

    public Vector GetTextureCoords(Vector p, float time)
    {
      return new Vector();
    }
  }
}
