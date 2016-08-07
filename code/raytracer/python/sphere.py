from linearAlgebra import *
from math import sqrt


class Sphere(object):
    def __init__(self, radius, position, materials):
        self.radius = radius
        self.position = position
        self.materials = materials

    # Checks intersection between ray and specific sphere
    def intersect(self, ray):
        a = dot(ray.direction, ray.direction)
        b = 2 * dot(sub(ray.position, self.position), ray.direction)
        c = dot(sub(ray.position, self.position), sub(ray.position, self.position)) - self.radius * self.radius

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