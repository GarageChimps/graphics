import sys
from linearAlgebra import *


class Face:
    def __init__(self, position_indices, tex_coord_indices=None, normal_indices=None, mesh=None):
        self.position_indices = position_indices
        self.tex_coord_indices = tex_coord_indices
        self.normal_indices = normal_indices
        self.mesh = mesh
        self.materials = mesh.materials
        self.face_normal = normalize(cross(sub(self.position(2), self.position(1)),
                                     sub(self.position(0), self.position(1))))
        self.barycentric_cache = None

    def coord(self, vertex, axis):
        return self.mesh.positions[self.position_indices[vertex]][axis]

    def position(self, vertex):
        return self.mesh.positions[self.position_indices[vertex]]

    def normal(self, vertex):
        if self.normal_indices is None or len(self.normal_indices) == 0:
            return self.face_normal
        return self.mesh.normals[self.normal_indices[vertex]]

    def get_normal(self, p):
        if self.barycentric_cache is None:
            assert False
        na = mult_scalar(self.barycentric_cache[0], self.normal(0))
        nb = mult_scalar(self.barycentric_cache[1], self.normal(1))
        nc = mult_scalar(self.barycentric_cache[2], self.normal(2))
        n = add(na, add(nb, nc))
        return normalize(n)

    def intersect(self, ray):
        xa = self.coord(0, 0)
        xb = self.coord(1, 0)
        xc = self.coord(2, 0)
        ya = self.coord(0, 1)
        yb = self.coord(1, 1)
        yc = self.coord(2, 1)
        za = self.coord(0, 2)
        zb = self.coord(1, 2)
        zc = self.coord(2, 2)
        xd = ray.direction[0]
        yd = ray.direction[1]
        zd = ray.direction[2]
        xe = ray.position[0]
        ye = ray.position[1]
        ze = ray.position[2]

        tMin = float("inf")

        detA = det(xa - xb, xa - xc, xd, ya - yb, ya - yc, yd, za - zb, za - zc, zd)
        t = det(xa - xb, xa - xc, xa - xe, ya - yb, ya - yc, ya - ye, za - zb, za - zc,
                za - ze) / detA
        if t < 0:
            return tMin, None
        gamma = det(xa - xb, xa - xe, xd, ya - yb, ya - ye, yd, za - zb, za - ze, zd) / detA
        if gamma < 0 or gamma > 1:
            return tMin, None
        beta = det(xa - xe, xa - xc, xd, ya - ye, ya - yc, yd, za - ze, za - zc, zd) / detA
        if beta < 0 or beta > (1 - gamma):
            return tMin, None
        tMin = t
        self.barycentric_cache = [1 - beta - gamma, beta, gamma]
        return tMin, self


class Mesh:
    def __init__(self, file_path=None, materials=None):
        self.file_path = file_path
        self.materials = materials

        self.positions = []
        self.normals = []
        self.tex_coordinates = []
        self.faces = []
        self.bounding_box = None

    def init(self):
        if self.file_path is not None:
            self._load_from_obj(self.file_path)
            self._create_bounding_box()

    def _create_bounding_box(self):
        self.bounding_box = Mesh()
        if len(self.positions) == 0:
            return
        min = [float("inf"), float("inf"), float("inf")]
        max = [float("-inf"), float("-inf"), float("-inf")]
        for p in self.positions:
            for i, v in enumerate(p):
                if v < min[i]:
                    min[i] = v
                if v > max[i]:
                    max[i] = v

        a, b = min, max
        self.bounding_box.positions = [
                     [a[0], a[1], a[2]], [a[0], a[1], b[2]], [a[0], b[1], a[2]],
                     [a[0], b[1], b[2]], [b[0], a[1], a[2]], [b[0], a[1], b[2]],
                     [b[0], b[1], a[2]], [b[0], b[1], b[2]]
                     ]
        self.bounding_box.faces = [
                                   Face([0, 2, 1], mesh=self.bounding_box),
                                   Face([1, 2, 3], mesh=self.bounding_box),
                                   Face([4, 5, 6], mesh=self.bounding_box),
                                   Face([5, 7, 6], mesh=self.bounding_box),
                                   Face([0, 1, 4], mesh=self.bounding_box),
                                   Face([4, 1, 5], mesh=self.bounding_box),
                                   Face([2, 6, 3], mesh=self.bounding_box),
                                   Face([3, 6, 7], mesh=self.bounding_box),
                                   Face([0, 4, 2], mesh=self.bounding_box),
                                   Face([4, 6, 2], mesh=self.bounding_box),
                                   Face([1, 3, 5], mesh=self.bounding_box),
                                   Face([3, 7, 5], mesh=self.bounding_box)
                                   ]
        self.bounding_box.mesh = self.bounding_box

    def _load_from_obj(self, file_path):
        with open(file_path) as f:
            content = f.readlines()
            for line in content:
                parts = line.split()
                if len(parts) == 0:
                    continue
                parts = [p.strip() for p in parts]
                if parts[0] == 'v':
                    self.positions.append([float(parts[i]) for i in range(1, 4)])
                elif parts[0] == 'vt':
                    self.tex_coordinates.append([float(parts[i]) for i in range(1, 3)])
                elif parts[0] == 'vn':
                    self.normals.append([float(parts[i]) for i in range(1, 4)])
                elif parts[0] == 'f':
                    position_indices = []
                    tex_coord_indices = []
                    normal_indices = []
                    for part in [parts[i] for i in range(1,4)]:
                        indices = part.split("/")
                        if len(indices) > 0 and indices[0] != "":
                            position_indices.append(int(indices[0]) - 1)
                        if len(indices) > 1 and indices[1] != "":
                            tex_coord_indices.append(int(indices[1]) - 1)
                        if len(indices) > 2 and indices[2] != "":
                            normal_indices.append(int(indices[2]) - 1)
                    self.faces.append(Face(position_indices, tex_coord_indices,
                                           normal_indices, mesh=self))

    def intersect(self, ray):
        if self.bounding_box is not None:
            tMin, _ = self.bounding_box.intersect(ray)
            if tMin == float("inf"):
                return float("inf"), None
        tMin = float("inf")
        intersectedFace = None
        for face in self.faces:
            t, f = face.intersect(ray)
            if t < tMin:
                tMin = t
                intersectedFace = f
        return tMin, intersectedFace


if __name__ == '__main__':
    file_path = 'sphere.obj'
    if len(sys.argv) > 1:
        file_path = sys.argv[1]
    mesh = Mesh()
    mesh.load_from_obj(file_path)