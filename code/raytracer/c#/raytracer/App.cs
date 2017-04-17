namespace raytracer
{
  class App
  {
    static void Main(string[] args)
    {
      var imageFile = "image.png";
      var resourcesFile = "resources.json";
      var sceneFile = "taller5/scene8.json";
      var width = 512;
      var height = 512;

      var index = 0;
      var maxReflectionRecursions = 1;
      var samplesPerPixel = 1;
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
        }
        index++;
      }
      var resources = Resources.LoadResources(resourcesFile);
      var scene = Scene.LoadScene(sceneFile);
      scene.SetMaxNumberOfReflections(maxReflectionRecursions);
      scene.SetSamplesPerPixel(samplesPerPixel);
      var image = Raytracer.RayTrace(scene, resources, width, height);
      Display.GenerateImage(image, width, height, imageFile);
    }
  }
}
