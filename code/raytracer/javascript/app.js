
function render()
{
    resources = loadResources(resources);
	
	var width = 128.0;
	var height = 128.0;
    var image = rayTrace(scene, width, height);

	display(image, width, height);
}

render();