#version 130
precision highp float;

in vec3 outNormal;
in vec3 outPosition;

out vec4 pixelColor;

uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform vec3 cameraPosition;
uniform vec3 materialColor;

//A sampler is an object used to get colors from a texture
uniform sampler2D textureSampler;

void main(void)
{ 
  //Use the sampler to get the texture color. We are using as an example the normal as tex coords
  vec4 texColor = texture(textureSampler, outNormal.xy);
  //As an example we will add the texColor to the materialColor in this shader.
  //If there is no texture set, then texColor will be 0,0,0 (black), so adding black wont change the color
  pixelColor = vec4(materialColor + texColor.rgb, 1);
}
