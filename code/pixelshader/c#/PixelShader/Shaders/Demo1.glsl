#version 130
//Demo: 1. Anatomy of a pixel shader

/*Attributes: different values for each pixel*/

//Pixel coords: i,j coord of this pixel
in vec2 pixelCoords;

//Color to paint this pixel (r,g,b,a)
out vec4 pixelColor;

/*Uniforms: same value for  all pixels*/

//Size of the canvas
uniform vec2 size

void main(void)
{ 
  pixelColor = vec4(1, pixelCoords.x/size.x, pixelCoords.y/size.y, 1);
}
