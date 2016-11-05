#version 130
      
precision highp float;

in vec3 inPosition;

uniform mat4x4 transformationMatrix;

void main(void)
{
	gl_Position = transformationMatrix * vec4(inPosition, 1);
}