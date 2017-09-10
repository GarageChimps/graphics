using System;
using System.Collections.Generic;

namespace raytracer
{
  class OctTreeNode
  {
    private Box _boundingBox;
    private List<OctTreeNode> _children = new List<OctTreeNode>();
    private List<Face> _faces = new List<Face>();

    public OctTreeNode(Box boundingBox)
    {
      _boundingBox = boundingBox;
    }

    public List<OctTreeNode> Children
    {
      get { return _children; }
    }

    public Box BoundingBox
    {
      get { return _boundingBox; }
    }

    public string PrintString()
    {
      var s = " Face Count: " + _faces.Count;
      var i = 0;
      foreach (var child in _children)
      {
        s += "\n " + i + "." + child.PrintString();
        i++;
      }
      return s;
    }

    public void Initialize(Mesh mesh)
    {
      foreach (var face in mesh.Faces)
      {
        AddFaceToTree(mesh, face);
      }
    }

    private void AddFaceToTree(Mesh mesh, Face face)
    {
      if (_children.Count == 0)
      {
        if (_boundingBox.HasPoint(mesh.Positions[face.PositionIndices[0]]) ||
          _boundingBox.HasPoint(mesh.Positions[face.PositionIndices[1]]) ||
          _boundingBox.HasPoint(mesh.Positions[face.PositionIndices[2]]))
        {
          _faces.Add(face);
        }
      }
      else
      {
        foreach (var child in _children)
        {
          child.AddFaceToTree(mesh, face);
        }
      }
    }

    public Tuple<float, IObject> Intersect(Ray ray, float tMin = float.PositiveInfinity, Face intersectedFace = null)
    {
      var result = _boundingBox.TestIntersection(ray);
      if (result.Item2)
      {
        if (_children.Count == 0)
        {
          foreach (var face in _faces)
          {
            var intersectResult = face.Intersect(ray);
            if (intersectResult.Item1 < tMin)
            {
              tMin = intersectResult.Item1;
              intersectedFace = face;
            }
          }
        }
        else
        {
          foreach (var node in _children)
          {
            var intersectResult = node.Intersect(ray, tMin, intersectedFace);
            if (intersectResult.Item1 < tMin)
            {
              tMin = intersectResult.Item1;
              intersectedFace = (Face)intersectResult.Item2;
            }
          }
        }
      }
      return new Tuple<float, IObject>(tMin, intersectedFace);
    }

    public static OctTreeNode GenerateOctTree(Box boundingBox, int currentLevel, int maxLevels)
    {
      var node = new OctTreeNode(boundingBox);
      if (currentLevel < maxLevels)
      {
        var boxes = node.BoundingBox.OctPartition();
        boxes.ForEach(b => node.Children.Add(GenerateOctTree(b, currentLevel + 1, maxLevels)));
      }
      return node;
    }
  }
}
