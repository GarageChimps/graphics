from linearAlgebra import *
#
# List of available BRDFs functions
#


# Constant BRDF, reflectance doesnt modify base color
def constant(n, l, v, params):
    return 1.0


# Lambert BRDF, models diffuse reflectance using lambertian model
def lambert(n, l, v, params):
    return max(0,dot(n,l))


# BlinnPhong BRDF, models specular reflectance using Blinn-Phong aproximation
def blinnPhong(n, l, v, params):
    h = normalize(add(v, l))
    return pow(max(0,dot(n,h)), params["shininess"])
