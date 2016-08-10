using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace raytracer
{
  class Scene
  {
    public static void LoadScene(string fileName)
    {
      var jsonString = File.ReadAllText(fileName);
      ObjectHook(JToken.Parse(jsonString));      
    }

    private static object ObjectHook(JToken token)
    {
      switch (token.Type)
      {
        case JTokenType.Object:

          var children = token.Children<JProperty>();
          var dic = children
                      .ToDictionary(prop => prop.Name,
                                    prop => ObjectHook(prop.Value));
          if (dic.ContainsKey("__type__"))
          {
            if (dic["__type__"].ToString() == "scene")
            {
              //ToDo
            }
            else if (dic["__type__"].ToString() == "camera")
            {
              var fov = Convert.ToSingle(dic["fov"]);
              var position = ((List<object>)dic["position"]).Select(Convert.ToSingle).ToList();
			  //ToComplete
            }
            else if (dic["__type__"].ToString() == "sphere")
            {
              //ToDo
            }
            else if (dic["__type__"].ToString() == "point_light")
            {
              //ToDo
            }
          }
          return dic;

        case JTokenType.Array:
          return token.Select(ObjectHook).ToList();

        default:
          return ((JValue)token).Value;
      }
    }
  }
}
