namespace raytracer
{
  class App
  {
    static void Main(string[] args)
    {
      var imageFile = "scene1.png";
      var resourcesFile = "resources.json";
      var sceneFile = "scene1.json";
      var width = 32;
      var height = 32;

      var index = 0;
      var maxReflectionRecursions = 3;
      var samplesPerPixel = 1;
      var bitResolution = 8;
      var dither = false;
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
          if (arg.Contains("-w"))
            width = int.Parse(args[index + 1]);
          if (arg.Contains("-h"))
            height = int.Parse(args[index + 1]);
          if (arg.Contains("-m"))
            maxReflectionRecursions = int.Parse(args[index + 1]);
          if (arg.Contains("-p"))
            samplesPerPixel = int.Parse(args[index + 1]);
          if (arg.Contains("-b"))
            bitResolution = int.Parse(args[index + 1]);
          if (arg.Contains("-d"))
            dither = bool.Parse(args[index + 1]);

        }
        index++;
      }
      var resources = Resources.LoadResources(resourcesFile);
      var scene = Scene.LoadScene(sceneFile);
      scene.SetMaxNumberOfReflections(maxReflectionRecursions);
      scene.SetSamplesPerPixel(samplesPerPixel);
      var image = Raytracer.RayTrace(scene, resources, width, height);
      Display.GenerateImage(image, width, height, imageFile, bitResolution, dither);
    }
  }
}
