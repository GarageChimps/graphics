from utils import jsonfile2obj

from camera import *
from sphere import *
from lights import *


class Scene(object):
    def __init__(self, maxReflectionRecursions, camera, background, spheres, pointLights, ambientLights):
        self.maxReflectionRecursions = maxReflectionRecursions
        self.camera = camera
        self.background = background
        self.spheres = spheres
        self.objects = spheres
        self.pointLights = pointLights
        self.lights = pointLights
        self.ambientLights = ambientLights


def sceneHook(obj):
    if '__type__' in obj:
        if obj['__type__'] == "scene":
            return Scene(obj["maxReflectionRecursions"], obj["camera"], obj["background"], obj["spheres"],
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