#version 130
      
precision highp float;

in vec3 inPosition;

uniform mat4x4 viewProjection;

void main(void)
{
	gl_Position = viewProjection * vec4(inPosition, 1);
}