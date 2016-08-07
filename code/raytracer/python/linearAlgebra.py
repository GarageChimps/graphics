from math import sqrt
#
# Linear algebra lib
#


def add(vector1, vector2):
    return [vector1[0] + vector2[0], vector1[1] + vector2[1], vector1[2] + vector2[2]]


def mult(vector1, vector2):
    return [vector1[0] * vector2[0], vector1[1] * vector2[1], vector1[2] * vector2[2]]


def mult_scalar(scalar, vector):
    return [scalar * vector[0], scalar * vector[1], scalar * vector[2]]


def sub(vector1, vector2):
    return [vector1[0] - vector2[0], vector1[1] - vector2[1], vector1[2] - vector2[2]]


def dot(vector1, vector2):
    return vector1[0]*vector2[0] + vector1[1]*vector2[1] + vector1[2]*vector2[2]


def magnitude(vector1):
    return sqrt(dot(vector1, vector1))


def normalize(vector1):
    mag = magnitude(vector1)
    if mag == 0:
        return [0,0,0]
    return [vector1[0]/mag, vector1[1]/mag, vector1[2]/mag]


def cross(vector1, vector2):
    crossVec = [0,0,0]
    crossVec[0] = vector1[1] * vector2[2] - vector2[1] * vector1[2]
    crossVec[1] = vector2[0] * vector1[2] - vector1[0] * vector2[2]
    crossVec[2] = vector1[0] * vector2[1] - vector2[0] * vector1[1]
    return crossVec


