namespace raytracer
{
  class App
  {
    static void Main(string[] args)
    {
      var resources = Resources.LoadResources("resources.json");
      var scene = Scene.LoadScene("scene3.json");
      var width = 512;
      var height = 512;
      var image = Raytracer.RayTrace(scene, resources, width, height);
      Display.GenerateImage(image, width, height);
    }
  }
}
