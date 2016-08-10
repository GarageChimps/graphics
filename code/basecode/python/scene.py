from utils import jsonfile2obj

def loadScene(sceneFile):
    return jsonfile2obj(sceneFile, object_hook=sceneHook)


def sceneHook(obj):
    if '__type__' in obj:
        if obj['__type__'] == "scene":
            #ToDo
        if obj['__type__'] == "camera":
			fov = obj["fov"]
			position = obj["position"]
        if obj['__type__'] == "sphere":
            #ToDo
        if obj['__type__'] == "point_light":
            #ToDo        
    return obj


