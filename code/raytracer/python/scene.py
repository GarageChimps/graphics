from utils import jsonfile2obj

from camera import *
from sphere import *
from lights import *


class Scene(object):
    def __init__(self, params, camera, objects, lights):
        self.params = params
        self.camera = camera
        self.objects = objects
        self.lights = lights

    def get_background_color(self):
        if "background_color" in self.params:
            return self.params["background_color"]
        return [0, 0, 0]

    def get_param(self, param_name):
        if param_name in self.params:
            return self.params[param_name]
        return None

    def get_ambient_lights(self):
        return [l for l in self.lights if isinstance(l, AmbientLight)]

    def get_shading_lights(self):
        return [l for l in self.lights if not isinstance(l, AmbientLight)]


def loadScene(sceneFile):
    return jsonfile2obj(sceneFile, object_hook=sceneHook)


def sceneHook(obj):
    if '__type__' in obj:
        if obj['__type__'] == "scene":
            return Scene(obj["params"], obj["camera"], obj["objects"],
                         obj["lights"])
        if obj['__type__'] == "camera":
            return Camera(obj["fov"], obj["position"], obj["up"], obj["target"], obj["near"])
        if obj['__type__'] == "sphere":
            return Sphere(obj["radius"], obj["position"], obj["materials"])
        if obj['__type__'] == "pointLight":
            return PointLight(obj["position"], obj["color"])
        if obj['__type__'] == "directionalLight":
            return DirectionalLight(obj["direction"], obj["color"])
        if obj['__type__'] == "ambientLight":
            return AmbientLight(obj["color"])
    return obj


