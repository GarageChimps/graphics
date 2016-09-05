import sys

from raytracer import rayTrace
from resources import loadResources
from scene import loadScene
from display import display


def render(resourcesFilePath, sceneFilePath, imageFilePath, width, height):
    resources = loadResources(resourcesFilePath)
    scene = loadScene(sceneFilePath)
    image = rayTrace(scene, resources, width, height)

    display(image, width, height, imageFilePath)

if __name__ == "__main__":
    imageFilePath = "image.png"
    width = 64
    height = 64
    resourcesFilePath = "../../_resources/resources.json"
    sceneFilePath = "../../_scenes/taller3/scene1.json"

    for index, arg in enumerate(sys.argv):
        if index + 1 < len(sys.argv):
            if "-i" in arg:
                imageFilePath = sys.argv[index + 1]
            if "-r" in arg:
                resourcesFilePath = sys.argv[index + 1]
            if "-s" in arg:
                sceneFilePath = sys.argv[index + 1]
            if "-w" in arg:
                width = int(sys.argv[index + 1])
            if "-h" in arg:
                height = int(sys.argv[index + 1])

    render(resourcesFilePath, sceneFilePath, imageFilePath, width, height)
