from utils import jsonfile2obj

from camera import *
from sphere import *
from lights import *


class Scene(object):
    def __init__(self, params, camera, objects, pointLights, ambientLights):
        self.params = params
        self.camera = camera
        self.objects = objects
        self.pointLights = pointLights
        self.lights = pointLights
        self.ambientLights = ambientLights

    def get_background_color(self):
        if "background_color" in self.params:
            return self.params["background_color"]
        return [0, 0, 0]

    def get_param(self, param_name):
        if param_name in self.params:
            return self.params[param_name]
        return None

def sceneHook(obj):
    if '__type__' in obj:
        if obj['__type__'] == "scene":
            return Scene(obj["params"], obj["camera"], obj["objects"],
                         obj["pointLights"], obj["ambientLights"])
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


def loadScene(sceneFile):
    return jsonfile2obj(sceneFile, object_hook=sceneHook)