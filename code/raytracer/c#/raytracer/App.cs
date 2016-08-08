namespace raytracer
{
  class App
  {
    static void Main(string[] args)
    {
      var resources = Resources.LoadResources("Resources/resources.json");
      var scene = Scene.LoadScene("Scene/test_scene_1.json");
      var width = 512;
      var height = 512;
      var image = Raytracer.RayTrace(scene, null, width, height);
      Display.GenerateImage(image, width, height);
    }
  }
}
