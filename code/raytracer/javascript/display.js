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

//Displays image using ASCII characters to represent intensity
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

//Reverses a string
function reverse_string(string)
{
	var reverse = "";
	for (var i=string.length - 1; i>=0; i--)
		reverse += string.charAt(i);
	return reverse;
}