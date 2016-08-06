import brdfs
from utils import jsonfile2obj

# The resources json file has the name of the brdfs, this methods associates the actual function instead


class Resources(object):
    def __init__(self, materials):
        self.materials = {m.name: m for m in materials}


class Material(object):
    def __init__(self, type, name, color=None, brdfParams=None, brdf=None, reflectivity=None):
        self.type = type
        self.name = name
        self.color = color
        self.brdfParams = brdfParams
        self.brdf = brdf
        self.reflectivity = reflectivity


def resourcesHook(obj):
    if '__type__' in obj:
        if obj['__type__'] == "material":
            if 'reflectivity' in obj:
                return Material(obj['type'], obj['name'], reflectivity=obj['reflectivity'])
            brdfFunc = getattr(brdfs, obj['brdf'])
            return Material(obj['type'], obj['name'], obj['color'], obj['brdfParams'], brdfFunc)
        if obj['__type__'] == "resources":
            return Resources(obj['materials'])
    return obj

def loadResources(resourcesFile):
    return jsonfile2obj(resourcesFile, object_hook=resourcesHook)