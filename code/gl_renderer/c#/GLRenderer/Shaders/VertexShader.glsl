#version 130
      
precision highp float;

in vec3 inPosition;
in vec3 inNormal;

out vec3 outNormal;
out vec3 outPosition;

uniform mat4x4 transformationMatrix;

void main(void)
{
	gl_Position = transformationMatrix * vec4(inPosition, 1);
	outNormal = inNormal;
	outPosition = inPosition;
}