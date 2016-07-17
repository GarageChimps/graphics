//The resources json file has the name of the brdfs, this methods associates the actual function instead
function loadResources(resources)
{
	for(var materialName in resources.materials)
	{
		var brdf_name = resources.materials[materialName].brdf;
		var functPtr = window[brdf_name];
		resources.materials[materialName].brdf = functPtr;
	}
	return resources;
}