from math import *
from collections import namedtuple

from linearAlgebra import *
#
# Camera defs
#
CameraBounds = namedtuple('CameraBounds', ['t', 'b', 'r', 'l'])


class Camera(object):
    def __init__(self, fov, position, up, target, near=0.1):
        self.fov = fov
        self.position = position
        self.up = up
        self.target = target
        self.near = near
        self.coordinateBasis = self.getCameraCoordinatesBasis()
        self.cameraBounds = None

    # Returns the left,right,top,bottom bounds of the near plane in camera space
    def setCameraBounds(self, width, height):
        t = (abs(self.near) * tan(((self.fov / 2.0) / 180.0) * pi))
        b = -t
        r = t * width / height
        l = -r
        self.cameraBounds = CameraBounds(t, b, r, l)

    # Transforms pixel coordinate from image space to camera space
    def pixelToCameraCoords(self, i, j, width, height):
        bounds = self.cameraBounds
        u = bounds.l + (bounds.r - bounds.l) * (i + 0.5) / width
        v = bounds.b + (bounds.t - bounds.b) * (j + 0.5) / height
        w = -self.near
        return [u, v, w]

    # Gets camera space coordinate basis
    def getCameraCoordinatesBasis(self):
        w = normalize(sub(self.position, self.target))
        u = normalize(cross(self.up, w))
        v = normalize(cross(w, u))
        return [u, v, w]

    # Convert vector in camera space (cameraCoords) into world space for camera
    def cameraToWorldCoords(self, cameraCoords):
        worldCoords = self.position
        for i in range(3):
            worldCoords = add(worldCoords, mult_scalar(cameraCoords[i], self.coordinateBasis[i]))
        return worldCoords

