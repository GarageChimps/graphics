//The resources json file has the name of the brdfs, this methods associates the actual function instead
function loadResources(resources)
{
	var materialDict = {};
	for(var i=0; i<resources.materials.length; i++)
	{
		materialDict[resources.materials[i].name] = resources.materials[i];
	}
	resources.materials = materialDict; 
	for(var materialName in resources.materials)
	{
		if(resources.materials[materialName].__type__ == "brdf_material")
		{ 
			var brdf_name = resources.materials[materialName].brdf;
			var functPtr = window[brdf_name];
			resources.materials[materialName].brdf = functPtr;
		}
	}

	resources.getBrdfMaterials = function(materialNames){
		var brdfMaterials = [];
		for(var i=0; i<materialNames.length; i++)
		{
			if(this.materials[materialNames[i]].__type__ == "brdf_material")
				brdfMaterials.push(this.materials[materialNames[i]]);
		}
		return brdfMaterials;
	};
	resources.getReflectiveMaterials = function(materialNames){
		var reflectiveMaterials = [];
		for(var i=0; i<materialNames.length; i++)
		{
			if(this.materials[materialNames[i]].__type__ == "reflective_material")
				reflectiveMaterials.push(this.materials[materialNames[i]]);
		}
		return reflectiveMaterials;
	};
	resources.getAmbientMaterials = function(materialNames){
		var ambientMaterials = [];
		for(var i=0; i<materialNames.length; i++)
		{
			if(this.materials[materialNames[i]].use_for_ambient)
				ambientMaterials.push(this.materials[materialNames[i]]);
		}
		return ambientMaterials;
	}
}