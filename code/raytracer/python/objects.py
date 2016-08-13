from linearAlgebra import *
from math import sqrt


class Sphere(object):
    def __init__(self, radius, position, materials):
        self.radius = radius
        self.position = position
        self.materials = materials

    # Checks intersection between ray and specific sphere
    def intersect(self, ray):
        a = 1#dot(ray.direction, ray.direction)
        subpos = sub(ray.position, self.position)
        b = 2 * dot(subpos, ray.direction)
        c = dot(subpos, subpos) - self.radius * self.radius

        discr = b * b - 4 * a * c
        if discr < 0.0:
            return float("inf")

        discr = sqrt(discr)
        t0 = (-b - discr) / (2 * a)
        t1 = (-b + discr) / (2 * a)

        tMin = min(t0, t1)
        if tMin < 0.0:
            return float("inf")

        return tMin