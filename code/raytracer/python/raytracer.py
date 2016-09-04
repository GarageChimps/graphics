from camera import *
from linearAlgebra import *

#
# Raytracer defs
#

Ray = namedtuple('Ray', ['position', 'direction'])

#Main raytracing function, recieves a scene and image size, and returns a rendered image
def rayTrace(scene, resources, width, height):
    scene.camera.setCameraBounds(width, height)
    image = createImage(scene, width, height)
    for i in range(width):
        for j in range(height):
            ray = generatePixelRay(scene.camera, i, j, width, height)
            image[i][j] = intersectAndShade(ray, scene, resources, 0)
    return image


#Initialize a matrix to store the image to be rendered
def createImage(scene, width, height):
    image = []
    for i in range(width):
        image.append([])
        for j in range(height):
            image[i].append(scene.get_background_color())
    return image


#Generates a pixel view ray from the pixel coordinates
def generatePixelRay(camera, i, j, width, height):
    cameraCoords = camera.pixelToCameraCoords(i, j, width, height)
    worldCoords = camera.cameraToWorldCoords(cameraCoords)
    pixelDirection = normalize(sub(worldCoords, camera.position))
    return Ray(camera.position, pixelDirection)



#For a given ray, tests objects intersection and calculate corresponding color
def intersectAndShade(ray, scene, resources, recursion):
    if recursion > scene.get_int_param("maxReflectionRecursions"):
        return scene.get_background_color()
    intersectResult = intersectAllObjects(ray, scene)
    tIntersect = intersectResult[0]
    indexIntersect = intersectResult[1]
    if tIntersect < float("inf"):
        return getRayColor(ray, tIntersect, scene.objects[indexIntersect], scene, resources, recursion)
    return scene.get_background_color()


#Check intersection between ray and all objects of the scene
def intersectAllObjects(ray, scene):
    tMin = float("inf")
    indexMin = -1
    for index, obj in enumerate(scene.objects):
        t = obj.intersect(ray)
        if t < tMin:
            tMin = t
            indexMin = index

    return [tMin, indexMin]

#Calculates the color gathered by this ray after intersecting an object
def getRayColor(ray, tIntersection, object, scene,resources, recursion):
    p = add(ray.position, mult_scalar(tIntersection, ray.direction))
    n = normalize(sub(p, object.position))
    return shade(p, n, ray.direction, object.materials, scene, resources, recursion)


#Performs the shading calculation for a point, based on material reflectances and lights illumination
def shade(p, n, d, materials, scene, resources, recursion):
    color = [0, 0, 0]
    color = add(color, getAmbientColor(materials, scene, resources))
    color = add(color, getShadingColor(p, n, d, materials, scene, resources, recursion))
    return color

#Returns the ambient color generated by the ambient illumination of the scene for a material
def getAmbientColor(materials, scene, resources):
    color = [0, 0, 0]
    ambient_materials = resources.get_ambient_materials(materials)

    for light in scene.get_ambient_lights():
        for material in ambient_materials:
            ambientColor = mult(light.color, material.color)
            color = add(color, ambientColor)
    return color


#Returns the shading color generated by the shading illumination of the scene for a material
def getShadingColor(p, n, d, materials, scene, resources, recursion):
    color = [0, 0, 0]
    brdf_materials = resources.get_brdf_materials(materials)
    reflective_materials = resources.get_reflective_materials(materials)

    v = normalize(sub(scene.camera.position, p))
    for light in scene.get_shading_lights():
        l = light.get_direction(p)
        #Direct illumination
        if not scene.get_param("enable_shadows") or not isInShadow(p, l, scene):
            lightColor = light.color
            for material in brdf_materials:
                brdfVal = material.brdf(n, l, v, material.brdfParams)
                materialColor = mult(lightColor, mult_scalar(brdfVal, material.color))
                color = add(color, materialColor)

        #Indirect illummination
        for material in reflective_materials:
            reflectionRay = getReflectionRay(p, n, d)
            rayColor = intersectAndShade(reflectionRay, scene, resources, recursion + 1)
            materialColor = mult_scalar(material.reflectivity, rayColor)
            color = add(color, materialColor)

    return color


#Generates a shadow ray for a given point p for light l
def generateShadowRay(p, l):
    q = add(p, mult_scalar(0.001, l))
    return Ray(q, l)


#Tests if a point p is in shadow for a given light l in the given scene
def isInShadow(p, l, scene):
    ray = generateShadowRay(p, l)
    intersectResult = intersectAllObjects(ray, scene)
    tIntersect = intersectResult[0]
    return tIntersect < float("inf")


#Gets the reflection ray in a point p with normal n based on original viewing direction d
def getReflectionRay(p, n, d):
    r = normalize(sub(d, mult_scalar(dot(d,n)*2, n)))
    q = add(p, mult_scalar(0.001, r))
    return Ray(q, r)

















