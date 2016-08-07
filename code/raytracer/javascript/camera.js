/**
 * Camera functions
 */

//Returns the left,right,top,bottom bounds of the near plane in camera space
function setCameraBounds(camera, width, height)
{
	var bounds = new Object();
	bounds.t = (Math.abs(camera.near) * Math.tan(((camera.fov / 2.0) / 180.0) * Math.PI));
	bounds.b = -bounds.t;
	bounds.r = bounds.t * width / height;
	bounds.l = -bounds.r;
	camera.cameraBounds = bounds;
}

//Transforms pixel coordinate from image space to camera space
function pixelToCameraCoords(camera, i, j, width, height)
{
	var bounds = camera.cameraBounds;
	var u = bounds.l + (bounds.r - bounds.l) * (i + 0.5) / width;
	var v = bounds.b + (bounds.t - bounds.b) * (j + 0.5) / height;
	var w = -camera.near;
	return [u,v,w];
}

//Gets camera space coordinate basis
function setCameraCoordinatesBasis(camera)
{
    var w = normalize(sub(camera.position, camera.target));
    var u = normalize(cross(camera.up, w));
    var v = normalize(cross(w, u));
    camera.coordinateBasis = [u,v,w]
}

//Convert vector in camera space (cameraCoords) into world space for camera
function cameraToWorldCoords(camera, cameraCoords)
{
	var basis = camera.coordinateBasis;
	var worldCoords = camera.position;
	for(var i=0; i<3; i++)
		worldCoords = add(worldCoords, mult_scalar(cameraCoords[i], basis[i]));
	return worldCoords;
}
