from raytracer import rayTrace
from resources import loadResources
from display import display


def render():
    resources = loadResources("resources/resources.json")
    scene = {}
    width = 512.0
    height = 512.0
    image = rayTrace(scene, width, height)

    display(image, width, height)

render()