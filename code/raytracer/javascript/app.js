
function render()
{
    resources = loadResources(resources);
	
	var width = 512.0;
	var height = 512.0;
    var image = rayTrace(scene, width, height);

	display(image, width, height);
}

render();