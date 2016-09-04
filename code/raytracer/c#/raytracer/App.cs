﻿namespace raytracer
{
  class App
  {
    static void Main(string[] args)
    {
      var imageFile = "image.png";
      var resourcesFile = "resources.json";
      var sceneFile = "taller1/scene1.json";
      var index = 0;
      foreach (var arg in args)
      {
        if (index + 1 < args.Length)
        {
          if (arg.Contains("-i"))
            imageFile = args[index + 1];
          if (arg.Contains("-r"))
            resourcesFile = args[index + 1];
          if (arg.Contains("-s"))
            sceneFile = args[index + 1];
        }
        index++;
      }
      var resources = Resources.LoadResources(resourcesFile);
      var scene = Scene.LoadScene(sceneFile);
      var width = 512;
      var height = 512;
      var image = Raytracer.RayTrace(scene, resources, width, height);
      Display.GenerateImage(image, width, height, imageFile);
    }
  }
}
