import numpy as np

from PySide.QtGui import *
from PySide.QtOpenGL import *

from OpenGL.GL import *

WIDTH = 512
HEIGHT = 512
PIXEL_SHADER_PATH = "PixelShader.glsl"
VERTEX_SHADER_PATH = "VertexShader.glsl"


class App(QApplication):
    def __init__(self, width, height):
        QApplication.__init__(self, "")
        self.width = width
        self.height = height

    def init_window(self):
        self.window = Window(self.width, self.height)
        self.window.show()


class Window(QMainWindow):
    def __init__(self, width, height):
        QMainWindow.__init__(self, None)
        self.setWindowTitle("GLRenderer")
        self.setFixedWidth(width)
        self.setFixedHeight(height)
        self.setCentralWidget(Renderer())


class Renderer(QGLWidget):

    def __init__(self):
        QGLWidget.__init__(self)
        self.pixelShaderSource = ""
        self.vertexShaderSource = ""

        # In OpenGL the way to identify resources in the GPU is with integer ids, which
        # are called "handles" in this code
        self.shaderProgramHandle = None
        self.transformationMatrixHandle = None
        self.lightPositionHandle = None
        self.lightColorHandle = None
        self.cameraPositionHandle = None
        self.materialColorHandle = None
        self.vertexPositionBufferHandle = None
        self.vertexNormalBufferHandle = None
        self.facesBufferHandle = None
        self.vertexPositionAttributeHandle = None
        self.objectHandle = None

        # TODO: Replace this with vertices position data loaded from the mesh in the scene
        self.vertexPositionData = [
            -1.0, -1.0, -1.0, #v1
             1.0, -1.0, -1.0, #v2
             1.0,  1.0, -1.0, #v3
            -1.0,  1.0, -1.0  #v4
        ]

        # TODO: Replace this with vertices normal data loaded from the mesh in the scene
        self.vertexNormalData = [
            1.0, 0.0, 0.0,  # v1
            0.0, 1.0, 0.0,  # v2
            0.0, 0.0, 1.0,  # v3
            1.0, 0.0, 1.0  # v4
        ]

        # TODO: Replace this with face data loaded from the mesh in the scene
        self.facesData = [
            0, 1, 2, #face1
            2, 3, 0  #face2
        ];

        # TODO: Replace this with the orthographic projection transformation matrix
        # without including the image transformation (P * C)
        # Column order
        self.transformationMatrix = [
            0.2, 0, 0, 0,
            0, 0.2, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ]

        #TODO: Replace this with scene data
        self.cameraPosition = [1, 0, 0]
        self.lightPosition = [0, 1, 0]
        self.lightColor = [0, 0, 1]
        self.materialColor = [1, 0, 0]

    # The initialization method in an OpenGL program is called once.
    # In the initialization method, we load and send the shaders to the GPU,
    # and also send all the geometry information
    def initializeGL(self):
        glEnable(GL_DEPTH_TEST)
        # TODO: Replace this with the background color of the scene
        glClearColor(1.0, 1.0, 1.0, 1.0)

        self._load_shaders()
        self._create_shaders()
        self._create_buffers()
        self._create_objects()

    def _load_shaders(self):
        with open(PIXEL_SHADER_PATH, 'r') as myfile:
            self.pixelShaderSource = myfile.read()
        with open(VERTEX_SHADER_PATH, 'r') as myfile:
            self.vertexShaderSource = myfile.read()

    # Shader source code is stored in GPU but we need to pass it to the GPU and instruct
    # it to compile the code and create the program that represents the rendering algorithm
    # that will be used
    def _create_shaders(self):
        vertexShaderHandle = glCreateShader(GL_VERTEX_SHADER)
        pixelShaderHandle = glCreateShader(GL_FRAGMENT_SHADER)

        glShaderSource(vertexShaderHandle, self.vertexShaderSource)
        glShaderSource(pixelShaderHandle, self.pixelShaderSource)

        glCompileShader(vertexShaderHandle)
        glCompileShader(pixelShaderHandle)

        print glGetShaderInfoLog(vertexShaderHandle)
        print glGetShaderInfoLog(pixelShaderHandle)

        self.shaderProgramHandle = glCreateProgram()
        glAttachShader(self.shaderProgramHandle, vertexShaderHandle)
        glAttachShader(self.shaderProgramHandle, pixelShaderHandle)
        glLinkProgram(self.shaderProgramHandle)

        self.transformationMatrixHandle = glGetUniformLocation(self.shaderProgramHandle, "transformationMatrix")
        self.lightPositionHandle = glGetUniformLocation(self.shaderProgramHandle, "lightPosition")
        self.lightColorHandle = glGetUniformLocation(self.shaderProgramHandle, "lightColor")
        self.cameraPositionHandle = glGetUniformLocation(self.shaderProgramHandle, "cameraPosition")
        self.materialColorHandle = glGetUniformLocation(self.shaderProgramHandle, "materialColor")

    # Create the buffers to store the geometry information
    def _create_buffers(self):
        # We create one "array buffer" to store the vertices
        self.vertexPositionBufferHandle = glGenBuffers(1)
        glBindBuffer(GL_ARRAY_BUFFER, self.vertexPositionBufferHandle)
        glBufferData(GL_ARRAY_BUFFER, np.array(self.vertexPositionData, dtype='float32'),
                     GL_STATIC_DRAW)

        # We create one "array buffer" to store the normals
        self.vertexNormalBufferHandle = glGenBuffers(1)
        glBindBuffer(GL_ARRAY_BUFFER, self.vertexNormalBufferHandle)
        glBufferData(GL_ARRAY_BUFFER, np.array(self.vertexNormalData, dtype='float32'),
                     GL_STATIC_DRAW)

        # We create one "element array buffer" to store the faces
        self.facesBufferHandle = glGenBuffers(1)
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, self.facesBufferHandle)
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, np.array(self.facesData, dtype='uint32'),
                     GL_STATIC_DRAW)

    # OpenGL has the concept of "Vertex Array Objects (VAO)" which are groups of geometries,
    # shaders and textures that are associated to one object that wants to be drawn
    # The idea of a VAO is to associate to one single handle a group of configurations
    # that tell the GPU which geometry we want to render, with what shader and what textures
    def _create_objects(self):
        self.objectHandle = glGenVertexArrays(1)
        glBindVertexArray(self.objectHandle)

        glBindBuffer(GL_ARRAY_BUFFER, self.vertexPositionBufferHandle)
        glEnableVertexAttribArray(0)
        glVertexAttribPointer(0, 3, GL_FLOAT, False, 0, None)
        glBindAttribLocation(self.shaderProgramHandle, 0, "inPosition")

        glBindBuffer(GL_ARRAY_BUFFER, self.vertexNormalBufferHandle)
        glEnableVertexAttribArray(1)
        glVertexAttribPointer(1, 3, GL_FLOAT, False, 0, None)
        glBindAttribLocation(self.shaderProgramHandle, 0, "inNormal")

        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, self.facesBufferHandle)

        # In general in OpenGl binding to 0 represents unbinding the last element bound
        glBindVertexArray(0)

    # The render method of an OpenGL program will be called once per frame
    # erasing the previous image and generating a new one
    def paintGL(self):
        glViewport(0, 0, WIDTH, HEIGHT)
        glClear(GL_DEPTH_BUFFER_BIT | GL_COLOR_BUFFER_BIT)

        # We need to indicate which shader to use before the drawing, and before sending
        # uniform data
        glUseProgram(self.shaderProgramHandle)
        glUniformMatrix4fv(self.transformationMatrixHandle, 1, False, np.array(self.transformationMatrix,
                                                                               dtype='float32'))
        glUniform3fv(self.lightPositionHandle, 1, np.array(self.lightPosition, dtype='float32'))
        glUniform3fv(self.lightColorHandle, 1, np.array(self.lightColor, dtype='float32'))
        glUniform3fv(self.cameraPositionHandle, 1, np.array(self.cameraPosition, dtype='float32'))
        glUniform3fv(self.materialColorHandle, 1, np.array(self.materialColor, dtype='float32'))

        # We bind to our object handle so the GPU knows which geometries to draw
        glBindVertexArray(self.objectHandle)

        # Draw elements tells the GPU to draw what type of geometry with the faces/vertex data
        # already stored in the GPU buffers
        glDrawElements(GL_TRIANGLES, len(self.facesData), GL_UNSIGNED_INT, None)


if __name__ == "__main__":
    app = App(WIDTH, HEIGHT)
    app.init_window()
    app.exec_()