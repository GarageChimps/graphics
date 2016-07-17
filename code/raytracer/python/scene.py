import brdfs
from utils import jsonfile2obj


class Scene(object):
    def __init__(self, maxReflectionRecursions, camera, background, spheres, pointLights, ambientLights):
        self.maxReflectionRecursions = maxReflectionRecursions
        self.camera = camera
        self.background = background
        self.spheres = spheres
        self.pointLights = pointLights
        self.ambientLights = ambientLights


class Camera(object):
    def __init__(self, fov, position, up, target, near):
        self.fov = fov
        self.position = position
        self.up = up
        self.target = target
        self.near = near

class Sphere(object):
    def __init__(self, radius, position, materials):
        self.radius = radius
        self.position = position
        self.materials = materials


class PointLight(object):
    def __init__(self, position, color):
        self.position = position
        self.color = color

class AmbientLight(object):
    def __init__(self, color):
        self.color = color


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
        if obj['__type__'] == "ambientLight":
            return AmbientLight(obj["color"])


def loadScene(sceneFile):
    return jsonfile2obj(sceneFile, object_hook=sceneHook)