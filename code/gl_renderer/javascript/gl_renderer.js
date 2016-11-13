var pixelShaderSource = "";
var vertexShaderSource = "";

var GL;

//In OpenGL the way to identify resources in the GPU is with integer ids, which
    //are called "handles" in this code
var shaderProgramHandle;
var lightPositionHandle;
var lightColorHandle;
var cameraPositionHandle;
var materialColorHandle;
var transformationMatrixHandle;
var vertexPositionBufferHandle;
var vertexNormalBufferHandle;
var facesBufferHandle;
var vertexPositionAttributeHandle;
var vertexNormalAttributeHandle;

// TODO: Replace this with vertices positions data loaded from the mesh in the scene
var vertexPositionData =
    [
      -1.0, -1.0, -1.0,
      1.0, -1.0, -1.0,
      1.0,  1.0, -1.0,
      -1.0, 1.0, -1.0
	];

// TODO: Replace this with vertices normals data loaded from the mesh in the scene
var vertexNormalData =
    [
      1.0, 0.0, -1.0,
      0.0, 1.0, 0.0,
      0.0,  0.0, 1.0,
      1.0, 0.0, 1.0
	];

//TODO: Replace this with face data loaded from the mesh in the scene
var facesData =
    [
      0, 1, 2, 2, 3, 0
	];

//TODO: Replace this with the orthographic projection transformation matrix
//without including the image transformation (P * C)
//Column order
var viewProjectionMatrix =
    [
      0.2, 0, 0, 0, 
	  0, 0.2, 0, 0, 
	  0, 0, 1, 0, 
	  0, 0, 0, 1
	];

//TODO: Replace this with scene parameters
var cameraPosition = [1, 0, 0];
var lightPosition = [0, 1, 0];
var lightColor = [0, 0, 1];
var materialColor = [1, 0, 0];

// The initialization method in an OpenGL program is called once.
// In the initialization method, we load and send the shaders to the GPU,
// and also send all the geometry information
function init()
{
	var width = 512.0;
	var height = 512.0;
    var canvas = document.createElement("canvas");
	canvas.width = width;
	canvas.height = height;
	document.body.appendChild(canvas);

	try 
	{
		GL = canvas.getContext("experimental-webgl");
		GL.viewportWidth = canvas.width;
		GL.viewportHeight = canvas.height;
	} catch (e) {
	}
	if (!GL) {
		alert("Could not initialise WebGL.");
	}

	GL.enable(GL.DEPTH_TEST);
	GL.clearColor(1, 1, 1, 1);

	loadShaders();
	createShaders();
	createBuffers();
}



function getShader(id) 
{
	var shaderScript = document.getElementById(id);
	if (!shaderScript) {
		return null;
	}
	var str = "";
	var k = shaderScript.firstChild;
	while (k) {
		if (k.nodeType == 3) {
			str += k.textContent;
		}
		k = k.nextSibling;
	}
	return str;
}	

function loadShaders()
{
	vertexShaderSource = getShader("vertex_shader");
	pixelShaderSource = getShader("pixel_shader");

}

// Shader source code is stored in GPU but we need to pass it to the GPU and instruct
// it to compile the code and create the program that represents the rendering algorithm
// that will be used
function createShaders()
{
	var vertexShaderHandle = GL.createShader(GL.VERTEX_SHADER);
	var pixelShaderHandle = GL.createShader(GL.FRAGMENT_SHADER);

	GL.shaderSource(vertexShaderHandle, vertexShaderSource);
	GL.shaderSource(pixelShaderHandle, pixelShaderSource);

	GL.compileShader(vertexShaderHandle);
	GL.compileShader(pixelShaderHandle);

	if (!GL.getShaderParameter(vertexShaderHandle, GL.COMPILE_STATUS)) {
		alert(GL.getShaderInfoLog(vertexShaderHandle));
	}

	if (!GL.getShaderParameter(pixelShaderHandle, GL.COMPILE_STATUS)) {
		alert(GL.getShaderInfoLog(pixelShaderHandle));
	}

	 shaderProgramHandle = GL.createProgram();
	 GL.attachShader(shaderProgramHandle, vertexShaderHandle);
	 GL.attachShader(shaderProgramHandle, pixelShaderHandle);
	 GL.linkProgram(shaderProgramHandle);
	 if (!GL.getProgramParameter(shaderProgramHandle, GL.LINK_STATUS)) {
		alert("Could not initialise shaders");
	 }
	 
	 vertexPositionAttributeHandle = GL.getAttribLocation(shaderProgramHandle, "inPosition");
	 vertexNormalAttributeHandle = GL.getAttribLocation(shaderProgramHandle, "inNormal");
	 GL.enableVertexAttribArray(vertexPositionAttributeHandle);
	 GL.enableVertexAttribArray(vertexNormalAttributeHandle);
	
	 transformationMatrixHandle = GL.getUniformLocation(shaderProgramHandle, "transformationMatrix");
	 lightPositionHandle = GL.getUniformLocation(shaderProgramHandle, "lightPosition");
	 lightColorHandle = GL.getUniformLocation(shaderProgramHandle, "lightColor");
	 cameraPositionHandle = GL.getUniformLocation(shaderProgramHandle, "cameraPosition");
	 materialColorHandle = GL.getUniformLocation(shaderProgramHandle, "materialColor");
	 
}

// Create the buffers to store the geometry information
function createBuffers()
{
	vertexPositionBufferHandle = GL.createBuffer();
	GL.bindBuffer(GL.ARRAY_BUFFER, vertexPositionBufferHandle);
	GL.vertexAttribPointer(vertexPositionAttributeHandle, 3, GL.FLOAT, false, 0, 0);
	GL.bufferData(GL.ARRAY_BUFFER,
		new Float32Array(vertexPositionData),
		GL.STATIC_DRAW);

	verteNormalBufferHandle = GL.createBuffer();
	GL.bindBuffer(GL.ARRAY_BUFFER, verteNormalBufferHandle);
	GL.vertexAttribPointer(vertexNormalAttributeHandle, 3, GL.FLOAT, false, 0, 0);
	GL.bufferData(GL.ARRAY_BUFFER,
		new Float32Array(vertexNormalData),
		GL.STATIC_DRAW);

	facesBufferHandle = GL.createBuffer();
	GL.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, facesBufferHandle);
	GL.bufferData(GL.ELEMENT_ARRAY_BUFFER,
		new Uint16Array(facesData),
		GL.STATIC_DRAW);
}

// The render method of an OpenGL program will be called once in this example
// To allow interactivity we need to call this method every 1/60 seconds
function render()
{
	GL.viewport(0, 0, GL.viewportWidth, GL.viewportHeight);
	GL.clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);

	// We need to indicate which shader to use before the drawing, and before sending
	// uniform data
	GL.useProgram(shaderProgramHandle);
	GL.uniformMatrix4fv(transformationMatrixHandle, false, new Float32Array(viewProjectionMatrix));
	GL.uniform3fv(lightPositionHandle, new Float32Array(lightPosition));
	GL.uniform3fv(lightColorHandle, new Float32Array(lightColor));
	GL.uniform3fv(cameraPositionHandle, new Float32Array(cameraPosition));
	GL.uniform3fv(materialColorHandle, new Float32Array(materialColor));

	GL.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, facesBufferHandle);
	
	// Draw elements tells the GPU to draw what type of geometry with the faces/vertex data
	// already stored in the GPU buffers
	GL.drawElements(GL.TRIANGLES, facesData.length, GL.UNSIGNED_SHORT, 0);
}

init();
render();