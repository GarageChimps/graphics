#version 130
precision highp float;

in vec3 outNormal;
in vec3 outPosition;

out vec4 pixelColor;

uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform vec3 cameraPosition;
uniform vec3 materialColor;

void main(void)
{
  //ToDo: Implement pixel shading
  pixelColor = vec4(materialColor, 1);
}
