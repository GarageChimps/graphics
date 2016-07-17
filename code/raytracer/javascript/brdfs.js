/** 
* List of avilable BRDFs functions
*/

//Constant BRDF, reflectane doesnt modify base color
var constant = function(n, l, v, params)
{
	return 1.0;
};

//Lambert BRDF, models diffuse reflectance
var lambert = function(n, l, v, params)
{
	return Math.max(0,dot(n,l));
};

//BlinnPhong BRDF, models specular reflectance using Blinn-Phong aproximation
var blinnPhong = function(n, l, v, params)
{
	var h = normalize(add(v, l));
	return Math.pow(Math.max(0,dot(n,h)), params.shininess);
};