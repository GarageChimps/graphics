/**
 * Linear algenra functions
 */

function add(vector1, vector2)
{
	return [vector1[0] + vector2[0], vector1[1] + vector2[1], vector1[2] + vector2[2]];
}

function mult(vector1, vector2)
{
	return [vector1[0] * vector2[0], vector1[1] * vector2[1], vector1[2] * vector2[2]];
}

function mult_scalar(scalar, vector)
{
	return [scalar * vector[0], scalar * vector[1], scalar * vector[2]];
}

function sub(vector1, vector2)
{
	return [vector1[0] - vector2[0], vector1[1] - vector2[1], vector1[2] - vector2[2]];
}

function dot(vector1, vector2)
{
	return vector1[0]*vector2[0] + vector1[1]*vector2[1] + vector1[2]*vector2[2];
}

function magnitude(vector1)
{
	return Math.sqrt(dot(vector1, vector1)); 
}

function normalize(vector1)
{
	var mag = magnitude(vector1);
	return [vector1[0]/mag, vector1[1]/mag, vector1[2]/mag];
}

function cross(vector1, vector2)
{
  var crossVec = [0,0,0];
  crossVec[0] = vector1[1] * vector2[2] - vector2[1] * vector1[2];
  crossVec[1] = vector2[0] * vector1[2] - vector1[0] * vector2[2];
  crossVec[2] = vector1[0] * vector2[1] - vector2[0] * vector1[1];
  return crossVec;
}

