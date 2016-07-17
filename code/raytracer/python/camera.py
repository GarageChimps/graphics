from math import *

from linearAlgebra import *
#
# Camera defs
#

# Returns the left,right,top,bottom bounds of the near plane in camera space
def getCameraBounds(camera, width, height):
    bounds = object()
    bounds.t = (abs(camera.near) * tan(((camera.fov / 2.0) / 180.0) * pi))
    bounds.b = -bounds.t
    bounds.r = bounds.t * width / height
    bounds.l = -bounds.r
    return bounds


# Transforms pixel coordinate from image space to camera space
def pixelToCameraCoords(camera, i, j, width, height):
    bounds = getCameraBounds(camera, width, height)
    u = bounds.l + (bounds.r - bounds.l) * (i + 0.5) / width
    v = bounds.b + (bounds.t - bounds.b) * (j + 0.5) / height
    w = -camera.near
    return [u,v,w]


# Gets camera space coordinate basis
def getCameraCoordinatesBasis(camera):
    w = normalize(sub(camera.position, camera.target))
    u = normalize(cross(camera.up, w))
    v = normalize(cross(w, u))
    return [u,v,w]


# Convert vector in camera space (cameraCoords) into world space for camera
def cameraToWorldCoords(cameraCoords, camera):
    basis = getCameraCoordinatesBasis(camera)
    worldCoords = camera.position
    for i in range(3):
        worldCoords = add(worldCoords, mult_scalar(cameraCoords[i], basis[i]))
    return worldCoords

