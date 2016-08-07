
function render()
{
    loadResources(resources);
	loadScene(scene);
	
	var width = 512.0;
	var height = 512.0;
    var image = rayTrace(scene, resources, width, height);

	display(image, width, height);
}

render();