#version 130
//Demo: 2. Time

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

void main(void)
{ 
  pixelColor = vec4(pixelCoords/size, 0.5 + 0.5*cos(time),1);  
}
