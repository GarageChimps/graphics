#version 130
precision highp float;

in vec3 outColor;

out vec4 pixelColor;

void main(void)
{ 
  pixelColor = vec4(outColor, 1);
}
