import brdfs
# The resources json file has the name of the brdfs, this methods associates the actual function instead


def loadResources(resources):
    for materialName in resources.materials:
        brdf_name = resources.materials[materialName].brdf
        functPtr = getattr(brdfs, brdf_name)
        resources.materials[materialName].brdf = functPtr

    return resources
