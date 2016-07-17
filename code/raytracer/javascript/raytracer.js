
function createScene()
{
	var scene = {
		maxReflectionRecursions: 3,
		camera : {
			fov: 90.0,
			position: [0,0,15],
			up: [0,1,0],
			target: [0,0,0],
			near: 0.1
		},
		background: [1,1,1],
		spheres: [
		 {
			 radius: 1,
			 position: [0,6,5],
			 materials: ["red_lambert", "white_bph_100"]
		 },
		 {
			 radius: 5,
			 position: [0,0,0],
			 materials: ["green_lambert"]
		 }
		],
		pointLights: [
		 {
			position: [0, 10, 10],
			color: [0.7, 0.7, 0.7]
		 }
		],
		directionalLights: [],
		ambientLights: [
		 {
			 color: [0.2, 0.2, 0.2]
		 }
		]
	};
	return scene;
}

function getCameraBounds(camera, width, height)
{
	var bounds = new Object();
	bounds.t = (Math.abs(camera.near) * Math.tan(((camera.fov / 2) / 180.0) * Math.PI));
	bounds.b = -bounds.t;
	bounds.r = bounds.t * width / height;
	bounds.l = -bounds.r;
	return bounds;
}

function pixelToCameraCoords(camera, i, j, width, height)
{
	var bounds = getCameraBounds(camera, width, height);
	var u = bounds.l + (bounds.r - bounds.l) * (i + 0.5) / width;
	var v = bounds.b + (bounds.t - bounds.b) * (j + 0.5) / height;
	var w = -camera.near;
	return [u,v,w];
}

function getCameraCoordinatesBasis(camera)
{
    var w = normalize(sub(camera.position, camera.target));
    var u = normalize(cross(camera.up, w));
    var v = normalize(cross(w, u));
	  return [u,v,w];
}

function cameraToWorldCoords(cameraCoords, camera)
{
	var basis = getCameraCoordinatesBasis(camera);
	var worldCoords = camera.position;
	for(var i=0; i<3; i++)
		worldCoords = add(worldCoords, ponder(cameraCoords[i], basis[i]));
	return worldCoords;
}

function generatePixelRay(camera, i, j, width, height)
{
	var cameraCoords = pixelToCameraCoords(camera, i, j, width, height);
	var worldCoords = cameraToWorldCoords(cameraCoords, camera);
	var pixelDirection = normalize(sub(worldCoords, camera.position));
	return {
		position : camera.position,
		direction : pixelDirection
	}
}


function intersectSphere(ray, sphere)
{
	  var a = dot(ray.direction, ray.direction);
	  var b = 2* dot(sub(ray.position, sphere.position), ray.direction);
	  var c = dot(sub(ray.position, sphere.position), sub(ray.position, sphere.position)) - sphere.radius*sphere.radius;

	  var discr = b*b - 4*a*c;
	  if(discr < 0.0)
			return Infinity;

	  discr = Math.sqrt(discr);
	  var t0 = (-b - discr) / (2*a);
	  var t1 = (-b + discr) / (2*a);

	  var tMin = Math.min(t0, t1);
	  if(tMin < 0.0)
			return Infinity;

	  return tMin;
}

function getAmbientColor(baseMaterial, scene)
{
	var color = [0,0,0];
	for(var i=0; i<scene.ambientLights.length; i++)
	{
		var ambientColor = mult(scene.ambientLights[i].color, baseMaterial.color);
		color = add(color, ambientColor);
	}
	return color;
}

function generateShadowRay(p, l)
{
	var q = add(p, ponder(0.001, l));
	return {
		position : q,
		direction : l
	}
}

function isInShadow(p, l, scene)
{
	var ray = generateShadowRay(p, l);
	var intersectResult = intersectAllObjects(ray, scene);
	var tIntersect = intersectResult[0];
	return tIntersect < Infinity;
}

function getReflectionRay(p, n, d)
{
	var r = normalize(sub(d, ponder(dot(d,n)*2, n)));
	var q = add(p, ponder(0.001, r));
	return {
		position : q,
		direction : r
	}
}

function shade(p, n, d, materials, scene, recursion)
{
	var color = getAmbientColor(resources.materials[materials[0]], scene)
	var v = normalize(sub(scene.camera.position, p));

	for(var i=0; i<scene.pointLights.length; i++)
	{
		var lightColor = scene.pointLights[i].color;
		var lightPos = scene.pointLights[i].position;
		var l = normalize(sub(lightPos, p));
		if(isInShadow(p, l, scene))
			lightColor = [0,0,0];

		for(var j=0; j<materials.length; j++)
		{
			var material = resources.materials[materials[j]];
			if(material.type == "brdf")
			{
				var brdfVal = material.brdf(n,l,v, material.brdfParams);
				var materialColor = mult(lightColor, ponder(brdfVal, material.color));
			}
			else if(material.type == "mirror")
			{
				var reflectionRay = getReflectionRay(p, n, d);
				var materialColor = intersectAndShade(reflectionRay, scene, recursion + 1);
			}
			color = add(color, materialColor);
		}

	}
	return color;
}



function calculatePixelColor(ray, tIntersection, sphere, scene, recursion)
{
	var p = add(ray.position, ponder(tIntersection, ray.direction));
	var n = normalize(sub(p, sphere.position));
	return shade(p, n, ray.direction, sphere.materials, scene, recursion);
}

function createImage(scene, width, height)
{
	var image = new Array(width);
	for(var i=0; i<width; i++)
	{
		image[i] = new Array(height);
		for(var j=0; j<height; j++)
		{
			image[i][j] = scene.background;
		}
	}
	return image;
}

function intersectAllObjects(ray, scene)
{
	var tMin = Infinity;
	var indexMin = -1;
	for(var index = 0; index < scene.spheres.length; index++)
	{
		var t = intersectSphere(ray, scene.spheres[index]);
		if (t < tMin)
		{
			tMin = t;
			indexMin = index;
		}
	}
	return [tMin, indexMin];
}

function intersectAndShade(ray, scene, recursion)
{
	if(recursion > scene.maxReflectionRecursions)
		return scene.background;
	var intersectResult = intersectAllObjects(ray, scene);
	var tIntersect = intersectResult[0];
	var indexIntersect = intersectResult[1];
	if(tIntersect < Infinity)
	 return calculatePixelColor(ray, tIntersect, scene.spheres[indexIntersect], scene, recursion);
	return scene.background;
}

function rayTrace(scene, width, height)
{
	var image = createImage(scene, width, height);
	for(var i=0; i<width; i++)
	{
		for(var j=0; j<height; j++)
		{
			var ray = generatePixelRay(scene.camera, i, j, width, height);
			image[i][j] = intersectAndShade(ray, scene, 0);
		}
	}
	return image;
}

function setColor(imageData, i, j, r, g, b)
{
	var index = (i * 4) + (imageData.height -  j - 1) * imageData.width * 4;
	var rInt = Math.floor(255.0*r);
	var gInt = Math.floor(255.0*g);
	var bInt = Math.floor(255.0*b);
	imageData.data[index + 0] = rInt;
	imageData.data[index + 1] = gInt;
	imageData.data[index + 2] = bInt;
	imageData.data[index + 3] = 255;
}

function display(image, width, height)
{
	var canv = document.createElement("canvas");
	canv.width = width;
	canv.height = height;
	document.body.appendChild(canv);

	var ctx = canv.getContext("2d");
	var imageData = ctx.getImageData(0, 0, width, height);

	for(var i=0; i<width; i++)
	{
		for(var j=0; j<height; j++)
		{
			var color = image[i][j];
			setColor(imageData, i, j, color[0], color[1], color[2]);
		}
	}
	ctx.putImageData(imageData, 0, 0);
}

function reverse_string(string)
{
	var reverse = "";
	for (var i=string.length - 1; i>=0; i--)
		reverse += string.charAt(i);
	return reverse;
}

function display_ascii(image, width, height)
{
	var characterMap = reverse_string(" .:-=+*#%@");
	//var characterMap = reverse_string("$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\|()1{}[]?-_+~<>i!lI;:,^`'. ");
	var canv = document.createElement("canvas");
	canv.width = width;
	canv.height = height;
	document.body.appendChild(canv);

	var charI = 0;
	var charJ = 0;
	var charHeight = 8;
	var ctx = canv.getContext("2d");
	ctx.font = "" + charHeight + "px Courier New";
	var charWidth = ctx.measureText('A').width;
	canv.width = width * charWidth;
	canv.height = height * charHeight;
		
	for(var i=0; i<width; i++)
	{
		for(var j=height - 1; j>=0; j-=2)
		{
			var color = image[i][j];
			var grayScale = (color[0] + color[1] + color[2])/3;
			var charIndex = Math.floor(characterMap.length*grayScale);
			var character = characterMap.charAt(charIndex); 
			charJ += charHeight;
			ctx.fillStyle = 'rgb(' + Math.floor(255*color[0]) + ',' +
									 Math.floor(255*color[1]) + ',' + 
									 Math.floor(255*color[2]) + ')';
			ctx.fillText(character, charI, charJ);
		}
		charI += charWidth;
		charJ = 0;
	}
	
}

function render()
{
	var width = 512;
	var height = 512;
	var scene = createScene();
	var image = rayTrace(scene, width, height);
	display_ascii(image, width, height);
}

render();
