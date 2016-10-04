#version 130
  
precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

uniform vec2 mouse;
uniform int key;
uniform vec2 size;
uniform float rand;

vec2 center = vec2(0.5,0.5);
float radius = 0.3;

vec4 background = vec4(0,0,0,1);
vec4 color1 = vec4(1,0,0,1);
vec4 color2 = vec4(0,0,0,1);

void main(void)
{ 
  center = size/2.0;
  radius = 0.4 * size.x;
  background = vec4(rand,rand,rand,1);
  pixelColor = background;
  float d = distance(pixelCoords, center);
  if (d < radius)
  {    
	float dToBrightness = distance(pixelCoords, mouse);	
	float interpolator = dToBrightness / radius;
	pixelColor = mix(color1, color2, interpolator);
  }
}
