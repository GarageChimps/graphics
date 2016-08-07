function loadScene(scene)
{
    setCameraCoordinatesBasis(scene.camera);
    scene.getBackgroundColor = function(){
        if ("background_color" in this.params)
            return this.params["background_color"]
        return [0,0,0]
    };
    scene.getParam = function(paramName){
        if (paramName in this.params)
            return this.params[paramName]
        return None
    };
    scene.getAmbientLights = function(){
        var ambientLights = [];
        for(var i=0; i<this.lights.length; i++)
        {
            if (this.lights[i].__type__ == "ambient_light")
                ambientLights.push(this.lights[i]);
        }
        return ambientLights;
    };
    scene.getShadingLights = function(){
        var shadingLights = [];
        for(var i=0; i<this.lights.length; i++)
        {
            if (this.lights[i].__type__ != "ambient_light")
                shadingLights.push(this.lights[i]);
        }
        return shadingLights;
    }
    for(var i=0; i<scene.lights.length; i++)
    {
        if (scene.lights[i].__type__ == "directional_light")
            scene.lights[i].getDirection = function(p){ return this.direction;}
        else if (scene.lights[i].__type__ == "point_light")
            scene.lights[i].getDirection = function(p){ return normalize(sub(this.position, p));}
    }
    for(var i=0; i<scene.objects.length; i++)
    {
        if (scene.objects[i].__type__ == "sphere")
        {
            scene.objects[i].intersect = intersectSphere;
        }
    }
}