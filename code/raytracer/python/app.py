from raytracer import rayTrace
from resources import loadResources
from scene import loadScene
from display import display


def render():
    resources = loadResources("../../_resources/resources.json")
    scene = loadScene("../../_scenes/scene3.json")
    width = 256
    height = 256
    image = rayTrace(scene, resources, width, height)

    display(image, width, height)

if __name__ == "__main__":
    render()
