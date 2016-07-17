
function render()
{
    resources = loadResources(resources);
	
	var width = 256;
	var height = 256;
    var image = rayTrace(scene, width, height);
    
	display(image, width, height);
}

render();