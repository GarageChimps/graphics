#version 130
//Mandelbrot
  
precision highp float;

in vec2 pixelCoords;
out vec4 pixelColor;

uniform vec2 mouse;
uniform vec2 size;
uniform float time;
uniform int key;

const int number_of_iterations = 100;

vec2 iteration(vec2 z, vec2 c)
{
	return vec2(z.x*z.x - z.y*z.y + c.x, 2*z.x*z.y + c.y);
}

void main(void)
{ 
  vec2 c = 4 * pixelCoords/size  - 2;  
  vec2 z = vec2(0);
  
  for(int it = 0; it < number_of_iterations; it++)
  {
	z = iteration(z, c);
  }

  float val = 1;
  if(length(z) < 2)
	 val = 0;
  pixelColor = vec4(val, val, val, 1);
}
