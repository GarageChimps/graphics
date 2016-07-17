from raytracer import rayTrace
from resources import loadResources
from scene import loadScene
from display import display

from utils import jsonfile2obj


def render():
    resources = loadResources("resources/resources.json")
    scene = loadScene("scenes/test_scene_1.json")
    width = 256
    height = 256
    image = rayTrace(scene, resources, width, height)

    display(image, width, height)

if __name__ == "__main__":
    render()