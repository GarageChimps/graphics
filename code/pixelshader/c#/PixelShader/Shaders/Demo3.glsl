#version 130
//Demo: 3. Simple 2D Circle

/*Attributes: different values for each pixel*/

//Pixel coords: i,j coord of this pixel
in vec2 pixelCoords;

//Color to paint this pixel (r,g,b,a)
out vec4 pixelColor;

/*Uniforms: same value for  all pixels*/

//Size of the canvas
uniform vec2 size;

//Running time of the program (> 0)
uniform float time;

/*Local Definitions*/
//Shader variables (local to this pixel)
vec2 center = vec2(256,256);
float radius = 200;
vec4 color = vec4(1,0,0,1);

void main(void)
{
  pixelColor = vec4(0,0,0,1);  
  if(distance(center, pixelCoords) < radius)
    pixelColor = color; 
}
