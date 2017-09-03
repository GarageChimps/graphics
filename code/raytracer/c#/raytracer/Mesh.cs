using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace raytracer
{
  class Face : IObject
  {
    private readonly Mesh _mesh;
    public Vector FaceNormal { get; }

    public List<int> PositionIndices { get; set; }
    public List<int> TexCoordIndices { get; set; }
    public List<int> NormalIndices { get; set; }
    public List<string> Materials { get; set; }

    public Face(List<int> positionIndices, List<int> texCoordIndices, List<int> normalIndices,  Mesh mesh)
    {
      _mesh = mesh;
      PositionIndices = positionIndices;
      TexCoordIndices = texCoordIndices;
      NormalIndices = normalIndices;
      Materials = _mesh.Materials;
      FaceNormal = ((GetPosition(2) - GetPosition(0)) % (GetPosition(0) - GetPosition(1))).Normalized;
    }

    public Tuple<float, IObject> Intersect(Ray ray)
    {
      float xa = GetPosition(0).X;
      float xb = GetPosition(1).X;
      float xc = GetPosition(2).X;
      float ya = GetPosition(0).Y;
      float yb = GetPosition(1).Y;
      float yc = GetPosition(2).Y;
      float za = GetPosition(0).Z;
      float zb = GetPosition(1).Z;
      float zc = GetPosition(2).Z;
      float xd = ray.Direction.X;
      float yd = ray.Direction.Y;
      float zd = ray.Direction.Z;
      float xe = ray.Position.X;
      float ye = ray.Position.Y;
      float ze = ray.Position.Z;

      float detA = Determinant(xa - xb, xa - xc, xd, ya - yb, ya - yc, yd, za - zb, za - zc, zd);
      float t = Determinant(xa - xb, xa - xc, xa - xe, ya - yb, ya - yc, ya - ye, za - zb, za - zc, za - ze) / detA;
      if (t < 0)
        return new Tuple<float,IObject>(float.PositiveInfinity, null);
      float gamma = Determinant(xa - xb, xa - xe, xd, ya - yb, ya - ye, yd, za - zb, za - ze, zd) / detA;
      if (gamma < 0 || gamma > 1)
        return new Tuple<float, IObject>(float.PositiveInfinity, null);
      float beta = Determinant(xa - xe, xa - xc, xd, ya - ye, ya - yc, yd, za - ze, za - zc, zd) / detA;
      if ((beta < 0) || (beta > (1 - gamma)))
        return new Tuple<float, IObject>(float.PositiveInfinity, null);

      return new Tuple<float, IObject>(t, this);
    }

    public Vector GetNormal(Vector p, float time)
    {
      var bar = BarycentricCoords(p);
      return (bar[0]*GetVertexNormal(0) + bar[1]*GetVertexNormal(1) + bar[2]*GetVertexNormal(2)).Normalized;
    }

    public Vector GetTextureCoords(Vector p, float time)
    {
      if (TexCoordIndices.Count == 0)
        return new Vector();
      var bar = BarycentricCoords(p);
      return bar[0] * _mesh.TexCoords[TexCoordIndices[0]] + bar[1] * _mesh.TexCoords[TexCoordIndices[1]] + bar[2] * _mesh.TexCoords[TexCoordIndices[2]];
    }

    public Vector GetVertexNormal(int vertex)
    {
      if (NormalIndices.Count == 0)
        return FaceNormal;
      return _mesh.Normals[NormalIndices[vertex]];
    }

    public Vector GetPosition(int vertex)
    {
      return _mesh.Positions[PositionIndices[vertex]];
    }

    private Vector BarycentricCoords(Vector p)
    {
      var a = GetPosition(0);
      var b = GetPosition(1);
      var c = GetPosition(2);
      var n = (b - a) % (c - a);
      var na = (c - b) % (p - b);
      var nb = (a - c) % (p - c);
      var nc = (b - a) % (p - a);
      var nDot = (n ^ n);
      var alpha = (n ^ na) / nDot;
      var beta = (n ^ nb) / nDot;
      var gamma = (n ^ nc) / nDot;
      return new Vector(alpha, beta, gamma);
    }

    private static float Determinant(float a, float b, float c, float d, float e, float f, float g, float h, float i)
    {
      return a * e * i - a * f * h - b * d * i + c * d * h + b * f * g - c * e * g;
    }

  }

  class Mesh : IObject
  {
    private readonly bool _computeVertexNormals;
    public string FilePath { get; set; }
    public List<string> Materials { get; set; }
    
    public List<Vector> Positions { get; private set; } 
    public List<Vector> Normals { get; private set; } 
    public List<Vector> TexCoords { get; private set; } 
    public List<Face> Faces { get; private set; } 

    public Mesh(string filePath, List<string> materials, bool computeVertexNormals)
    {
      _computeVertexNormals = computeVertexNormals;
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
            Positions.Add(new Vector(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
          else if (parts[0] == "vt")
            TexCoords.Add(new Vector(float.Parse(parts[1]), float.Parse(parts[2]), 0));
          else if (parts[0] == "vn")
            Normals.Add(new Vector(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
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
      var tMin = float.PositiveInfinity;
      Face intersectedFace = null;
      foreach (var face in Faces)
      {
        var intersectResult = face.Intersect(ray);
        if (intersectResult.Item1 < tMin)
        {
          tMin = intersectResult.Item1;
          intersectedFace = face;
        }
      }
      return new Tuple<float, IObject>(tMin, intersectedFace);
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
