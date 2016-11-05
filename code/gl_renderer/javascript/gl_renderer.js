var pixelShaderSource = "";
var vertexShaderSource = "";

var GL;
var shaderProgramHandle;
var viewProjectionHandle;
var vertexPositionBufferHandle;
var indicesBufferHandle;
var vertexPositionAttributeHandle;

var vertexPositionData =
    [
      -1.0, -1.0, -1.0,
      1.0, -1.0, -1.0,
      1.0,  1.0, -1.0,
      -1.0, 1.0, -1.0
	];

var indicesData =
    [
      0, 1, 2, 2, 3, 0
	];

//Column order
var viewProjectionMatrix =
    [
      0.2, 0, 0, 0, 
	  0, 0.2, 0, 0, 
	  0, 0, 1, 0, 
	  0, 0, 0, 1
	];

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
	GL.clearColor(0, 0.5, 0.5, 1);

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
	 GL.useProgram(shaderProgramHandle);

	 viewProjectionHandle = GL.getUniformLocation(shaderProgramHandle, "viewProjection");
	 vertexPositionAttributeHandle = GL.getAttribLocation(shaderProgramHandle, "inPosition");
	 GL.enableVertexAttribArray(vertexPositionAttributeHandle);
}

function createBuffers()
{
	vertexPositionBufferHandle = GL.createBuffer();
	GL.bindBuffer(GL.ARRAY_BUFFER, vertexPositionBufferHandle);
	GL.bufferData(GL.ARRAY_BUFFER,
		new Float32Array(vertexPositionData),
		GL.STATIC_DRAW);

	indicesBufferHandle = GL.createBuffer();
	GL.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, indicesBufferHandle);
	GL.bufferData(GL.ELEMENT_ARRAY_BUFFER,
		new Uint16Array(indicesData),
		GL.STATIC_DRAW);
}


function render()
{
	GL.viewport(0, 0, GL.viewportWidth, GL.viewportHeight);
	GL.clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);

	GL.uniformMatrix4fv(viewProjectionHandle, false, new Float32Array(viewProjectionMatrix));

	GL.bindBuffer(GL.ARRAY_BUFFER, vertexPositionBufferHandle);
	GL.vertexAttribPointer(vertexPositionAttributeHandle, 3, GL.FLOAT, false, 0, 0);
	

	GL.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, indicesBufferHandle);
	GL.drawElements(GL.TRIANGLES, indicesData.length, GL.UNSIGNED_SHORT, 0);
}

init();
render();