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
const vec3 colorIn = vec3(1,0.75,0.5);
const vec3 colorOut = vec3(0,0.5,1);

vec3 palette( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b*cos( 6.28318*(c*t+d) );
}

vec2 iteration(vec2 z, vec2 c)
{
	return vec2(z.x*z.x - z.y*z.y + c.x, 2*z.x*z.y + c.y);
}

void main(void)
{ 
  vec2 c = 4 * pixelCoords/size  - 2;
  float zoom = sqrt(1/pow((time + 1),1));
  vec2 mouseCorrected = 4 * mouse/size - 2;
  
  c = (c - mouseCorrected) * zoom;	
  
  vec3 paletteColor = vec3(0.0, 0.1, 0.2);
  if(key > 0)
     paletteColor = vec3(0.2, 0.1, 0.4);

  vec2 z = vec2(0);
  
  float finalIt = number_of_iterations;
  for(int it = 0; it < number_of_iterations; it++)
  {
	z = iteration(z, c);
	if(length(z) > 2)
		finalIt = it;
  }

  float interpolator = (number_of_iterations - finalIt)/number_of_iterations;
  vec3 color = palette(interpolator, vec3(0.5, 0.5, 0.5), vec3(0.5, 0.5, 0.5), vec3(1.0, 1.0, 1.0), paletteColor); 
  pixelColor = vec4(color, 1);
}
