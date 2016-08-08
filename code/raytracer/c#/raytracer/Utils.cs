using System.Linq;
using Newtonsoft.Json.Linq;

namespace raytracer
{
  
  public static class Utils
  {
    public static object Deserialize(string json)
    {
      var token = JToken.Parse(json);
      return ToObject(token);
    }

    private static object ToObject(JToken token)
    {
      switch (token.Type)
      {
        case JTokenType.Object:
          return token.Children<JProperty>()
                      .ToDictionary(prop => prop.Name,
                                    prop => ToObject(prop.Value));

        case JTokenType.Array:
          return token.Select(ToObject).ToList();

        default:
          return ((JValue)token).Value;
      }
    }
  }
}
