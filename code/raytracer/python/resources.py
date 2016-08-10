import brdfs
from utils import jsonfile2obj

# The resources json file has the name of the brdfs, this methods associates the actual function instead


class Resources(object):
    def __init__(self, materials):
        self.materials = {m.name: m for m in materials}

    def get_brdf_materials(self, material_names):
        return [self.materials[m] for m in material_names if isinstance(self.materials[m], BRDFMaterial)]

    def get_reflective_materials(self, material_names):
        return [self.materials[m] for m in material_names if isinstance(self.materials[m], ReflectiveMaterial)]

    def get_ambient_materials(self, material_names):
        return [self.materials[m] for m in material_names if isinstance(self.materials[m], BRDFMaterial) and
                self.materials[m].use_for_ambient]


class BRDFMaterial(object):
    def __init__(self, name, color, brdfParams, brdf, use_for_ambient):
        self.name = name
        self.color = color
        self.brdfParams = brdfParams
        self.brdf = brdf
        self.use_for_ambient = use_for_ambient


class ReflectiveMaterial(object):
    def __init__(self, name, reflectivity):
        self.name = name
        self.reflectivity = reflectivity


def loadResources(resourcesFile):
    return jsonfile2obj(resourcesFile, object_hook=resourcesHook)


def resourcesHook(obj):
    if '__type__' in obj:
        if obj['__type__'] == "brdf_material":
            brdfFunc = getattr(brdfs, obj['brdf'])
            use_for_ambient = False
            if 'use_for_ambient' in obj:
                use_for_ambient = obj['use_for_ambient']
            return BRDFMaterial(obj['name'], obj['color'], obj['brdfParams'], brdfFunc, use_for_ambient)
        if obj['__type__'] == "reflective_material":
            return ReflectiveMaterial(obj['name'], obj['reflectivity'])
        if obj['__type__'] == "resources":
            return Resources(obj['materials'])
    return obj

