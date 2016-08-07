from linearAlgebra import *


class PointLight(object):
    def __init__(self, position, color):
        self.position = position
        self.color = color

    def get_direction(self, p):
        return normalize(sub(self.position, p))


class DirectionalLight(object):
    def __init__(self, direction, color):
        self.direction = direction
        self.color = color

    def get_direction(self, p):
        return self.direction


class AmbientLight(object):
    def __init__(self, color):
        self.color = color