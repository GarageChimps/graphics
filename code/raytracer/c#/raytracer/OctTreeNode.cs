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
        AddFaceToTree(face);
    }

    private void AddFaceToTree(Face face)
    {
      if (_children.Count == 0)
      {
        if (TestBoxFaceIntersection(face))
          _faces.Add(face);
      }
      else
      {
        foreach (var child in _children)
          child.AddFaceToTree(face);
      }
    }

    //Separating Axis Theorem Test, implementation based on this: https://stackoverflow.com/questions/17458562/efficient-aabb-triangle-intersection-in-c-sharp
    //More on SAT: http://www.dyn4j.org/2010/01/sat/
    private bool TestBoxFaceIntersection(Face face)
    {
      // Test the box normals (x-, y- and z-axes)
      var index = 0;
      var boxAxes = _boundingBox.GetAxes();
      foreach (var axis in boxAxes)
      {
        var result = Project(face.GetPositions(), axis);
        var faceMax = result.Item2;
        var faceMin = result.Item1;
        if (faceMax < _boundingBox.Min[index] || faceMin > _boundingBox.Max[index])
          return false; // No intersection possible.
        index++;
      }

      // Test the triangle normal
      double faceOffset = face.FaceNormal ^ face.GetPosition(0);
      var result2 = Project(_boundingBox.GetCorners(), face.FaceNormal);
      var boxMax = result2.Item2;
      var boxMin = result2.Item1;
      if (boxMax < faceOffset || boxMin > faceOffset)
        return false; // No intersection possible.

      var edges = face.GetEdges();
      for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
          // The box normals are the same as it's edge tangents
          Vector axis = edges[i] % (boxAxes[j]);
          var result3 = Project(_boundingBox.GetCorners(), axis);
          var result4 = Project(face.GetPositions(), axis);
          var boxMax3 = result3.Item2;
          var boxMin3 = result3.Item1;
          var faceMax4 = result4.Item2;
          var faceMin4 = result4.Item1;
          if (boxMax3 < faceMin4 || boxMin3 > faceMax4)
            return false; // No intersection possible
        }

      return true;
    }

    private Tuple<float, float> Project(IEnumerable<Vector> points, Vector axis)
    {
      float min = float.PositiveInfinity;
      float max = float.NegativeInfinity;
      foreach (var p in points)
      {
        float val = axis ^ (p);
        if (val < min) min = val;
        if (val > max) max = val;
      }
      return new Tuple<float, float>(min, max);
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
