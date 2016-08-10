/**
 * Display functions, manage showing rendered image in html page
 */

//Basic display function, render image as pixels
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

//Sets an r,g,b color into the canvas imageData
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

