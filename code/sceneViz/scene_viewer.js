/*global THREE, requestAnimationFrame, Detector, dat */
if (!Detector.webgl) { Detector.addGetWebGLMessage(); }

var SCREEN_WIDTH = window.innerWidth;
var SCREEN_HEIGHT = window.innerHeight;

var container, camera3js, scene3js, renderer;

var cameraControls;
var controller;
var clock = new THREE.Clock();

init();
animate();


function createArrow(position, direction, radius, length, color)
{    
    var arrowGroup = new THREE.Object3D();
    var material = new THREE.MeshBasicMaterial( { color: color } );
    var axis = new THREE.Mesh(
        new THREE.CylinderGeometry( radius, radius, length, 48, 1, true ), material );
    arrowGroup.add( axis );
    
    var arrowPoint = new THREE.Mesh(
        new THREE.CylinderGeometry( 0, 2*radius, 4*radius, 48, 1, true ), material );
    arrowGroup.add( arrowPoint );

    updateArrow(arrowGroup, position, direction, radius, length, true);
    return arrowGroup;
}

function updateArrow(arrow, position, direction, radius, length, show)
{    
    var scale = show ? 1 : 0;
    var axis = arrow.children[0];
    var axisPosition = add(position, mult_scalar(length/2, direction));
    axis.position.set(axisPosition[0], axisPosition[1], axisPosition[2]);
    axis.scale.set(scale,scale,scale);
    updateObjectOrientation(axis, direction, [0,1,0]);

    var arrowPoint = arrow.children[1];
    var pointPosition = add(position, mult_scalar(length + radius * 2, direction));
    arrowPoint.position.set(pointPosition[0], pointPosition[1], pointPosition[2]);
    arrowPoint.scale.set(scale,scale,scale);
    updateObjectOrientation(arrowPoint, direction, [0,1,0]);
    
}

function updateObjectOrientation(obj, direction, up)
{
    var vectorDirection = new THREE.Vector3(direction[0], direction[1], direction[2]);
    vectorDirection = vectorDirection.normalize();
    var up = new THREE.Vector3(up[0], up[1], up[2]);
    obj.quaternion.setFromUnitVectors(up, vectorDirection);
    //obj.quaternion.setFromAxisAngle(vectorDirection, 0);
}

function toVector3(array)
{
    return new THREE.Vector3(array);
}

function updateObjectOrientationUsingBasis(obj, basisVectors)
{
    var m = new THREE.Matrix4();
    m.identity();
    m.set(basisVectors[0][0], basisVectors[0][1], basisVectors[0][2], 0, 
          basisVectors[1][0], basisVectors[1][1], basisVectors[1][2], 0,
          basisVectors[2][0], basisVectors[2][1], basisVectors[2][2], 0, 
          0, 0, 0, 1);
    //m.identity();
    //m.makeBasis(toVector3(basisVectors[0]), toVector3(basisVectors[1]), toVector3(basisVectors[2]));
    //m = m.transpose();
    m = m.getInverse(m);
    obj.quaternion.setFromRotationMatrix(m);
}

function init3jsObjects()
{
    var cameraMaterial = new THREE.MeshBasicMaterial( { color: 0x040404, wireframe: false, transparent: true, opacity: 0.8  } );
    var cameraGroup = new THREE.Object3D();
    var boxObj = new THREE.Mesh(new THREE.CubeGeometry( 2, 2, 4 ), cameraMaterial );
    cameraGroup.add(boxObj);
    var lensObj = new THREE.Mesh(new THREE.CylinderGeometry( 0, 1, 2, 48, 1, true ), cameraMaterial );
    lensObj.rotation.x = Math.PI / 2;
    lensObj.position.z = -2;
    cameraGroup.add(lensObj);
    var targetMaterial = new THREE.MeshBasicMaterial( { color: 0x040404, wireframe: true, transparent: true, opacity: 0.8  } );
    var targetObj = new THREE.Mesh(
        new THREE.SphereGeometry( 0.2, 16, 8 ), targetMaterial );
    var upObj = createArrow(scene.camera.position, scene.camera.up, 0.07, 2, 0x000000);
    var cameraU = createArrow(scene.camera.position, scene.camera.coordinateBasis[0], 0.07, 2, 0xff0000);
    var cameraV = createArrow(scene.camera.position, scene.camera.coordinateBasis[1], 0.07, 2, 0x00ff00);
    var cameraW = createArrow(scene.camera.position, scene.camera.coordinateBasis[2], 0.07, 2, 0x0000ff);

    var fustrumArrows = [];
    var fustrumCorners = getFustrumCorners(scene.camera);
    for(var i=0; i<4; i++)
    {
        var dir = normalize(sub(fustrumCorners[i], scene.camera.position));
        fustrumArrows.push(createArrow(scene.camera.position, dir, 0.01, 8000, 0x000000));
    }

    scene.camera.visualObject =  {"material": cameraMaterial, "obj": cameraGroup, "target": targetObj, "up": upObj,
                                  "coords": [cameraU, cameraV, cameraW], "fustrum": fustrumArrows};
    
    var imageObj = new THREE.Mesh(
        new THREE.PlaneGeometry( 1, 1, scene.image.width, scene.image.height ),
        new THREE.MeshBasicMaterial( { color: 0x0, wireframe: true } ) );
    var imageCoords = cameraToWorldCoords(scene.camera, [0,0,-scene.camera.near]);
    imageObj.position.set(imageCoords[0], imageCoords[1], imageCoords[2]);
    var imageOrigin = cameraToWorldCoords(scene.camera, pixelToCameraCoords(scene.camera, 0, 0, scene.image.width, scene.image.height));    
    var imageX = createArrow(imageOrigin, scene.camera.coordinateBasis[0], 0.03, 1, 0xff0000);
    var imageY = createArrow(imageOrigin, scene.camera.coordinateBasis[1], 0.03, 1, 0x00ff00);
    var rays = [];
    for(var i=0; i<scene.image.width; i++)
	{
		for(var j=0; j<scene.image.height; j++)
		{
			var ray = generatePixelRay(scene.camera, i, j, scene.image.width, scene.image.height);
            rays.push(createArrow(ray.position, ray.direction, 0.02, 8000, 0x222222));
        }
    }
    scene.image.visualObject = {"obj": imageObj, "coords": [imageX, imageY], "rays": rays};

    for(var i=0; i<scene.lights.length; i++)
    {
        var lightMaterial = new THREE.MeshLambertMaterial( { emissive: 0xFFFF00 } );
        var lightObj = new THREE.Mesh(new THREE.SphereGeometry( 1, 32, 16 ), lightMaterial );
        var light3js = new THREE.PointLight(0xffffff, 1.0);    
        scene.lights[i].visualObject = {"material": lightMaterial, "obj": lightObj, "light3js": light3js};
    }

    for(var i=0; i<scene.objects.length; i++)
    {
        var color = resources.materials[scene.objects[i].materials[0]].color;
        var sphereMaterial = new THREE.MeshLambertMaterial( { color: new THREE.Color(color[0],color[1],color[2]) } );
        var sphere = new THREE.Mesh(new THREE.SphereGeometry( 1, 32, 16 ), sphereMaterial );
        scene.objects[i].visualObject = {"material": sphereMaterial, "obj": sphere};
    }

    var x = createArrow([0,0,0], [1,0,0], 0.07, 2, 0xff0000);
    var y = createArrow([0,0,0], [0,1,0], 0.07, 2, 0x00ff00);
    var z = createArrow([0,0,0], [0,0,1], 0.07, 2, 0x0000ff);
    scene.visualObject = {"coords": [x,y,z]};
}

function init() {

    loadResources(resources);
	loadScene(scene);
    scene.image = {"width": 9, "height": 6};
    setCameraBounds(scene.camera, scene.image.width, scene.image.height);
    init3jsObjects();

    container = document.createElement('div');
    document.body.appendChild(container);

    // CAMERA
    camera3js = new THREE.PerspectiveCamera(scene.camera.fov, window.innerWidth / window.innerHeight, 1, 80000);
    camera3js.position.set(scene.camera.position[0], scene.camera.position[1], scene.camera.position[2]);

    // RENDERER
    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
    renderer.setClearColor(0xAAAAAA, 1.0);
    container.appendChild(renderer.domElement);

    renderer.gammaInput = true;
    renderer.gammaOutput = true;

    // EVENTS
    window.addEventListener('resize', onWindowResize, false);
    document.addEventListener('keydown', onKeyDown, false);
    document.addEventListener('keyup', onKeyUp, false);

    // CONTROLS
    cameraControls = new THREE.TrackballControls(camera3js);//, renderer.domElement);
    cameraControls.zoomSpeed = 0.1;
    cameraControls.rotateSpeed = 2.0;
    cameraControls.panSpeed = 0.3;
    cameraControls.target.set(0, 0, 0);

    fillScene();
    
    // GUI
    setupGui();
}

// EVENT HANDLERS

function onWindowResize() {

    SCREEN_WIDTH = window.innerWidth;
    SCREEN_HEIGHT = window.innerHeight;

    renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);

    camera3js.aspect = SCREEN_WIDTH / SCREEN_HEIGHT;
    camera3js.updateProjectionMatrix();

}

function onKeyDown(event) {

    switch (event.keyCode) {

        case 38: // up	
            break;
    }

}

function onKeyUp(event) {

    switch (event.keyCode) {

    }

}

function setupGui() {

    controller = {

        dummy: function () {
        }

    };

    var h;

    var gui = new dat.GUI();

    controller.cx = scene.camera.position[0];
    controller.cy = scene.camera.position[1];
    controller.cz = scene.camera.position[2];

    controller.tx = scene.camera.target[0];
    controller.ty = scene.camera.target[1];
    controller.tz = scene.camera.target[2];

    controller.upx = scene.camera.up[0];
    controller.upy = scene.camera.up[1];
    controller.upz = scene.camera.up[2];

    controller.fov = scene.camera.fov;
    controller.near = scene.camera.near;
    controller.cameraCoords = false;

    controller.fustrum = false;
    controller.imageCoords = false;
    controller.rays = false;
    controller.worldCoords = false;
    controller.setCamera = function() 
    {  
        cameraControls.reset();
        cameraControls.target.set(scene.camera.target[0], scene.camera.target[1], scene.camera.target[2]);
        camera3js.position.set(scene.camera.position[0], scene.camera.position[1], scene.camera.position[2]);
        camera3js.lookAt.set(scene.camera.target[0], scene.camera.target[1], scene.camera.target[2]);
        camera3js.up.set(scene.camera.up[0], scene.camera.up[1], scene.camera.up[2]);
        camera3js.fov = scene.camera.fov;
        camera3js.updateProjectionMatrix();
    };

    h = gui.addFolder("Scene parameters");
    h.add(controller, "worldCoords", false).name("coords");

    h = gui.addFolder("Image parameters");
    h.add(controller, "imageCoords", false).name("coords");
    h.add(controller, "rays", false).name("rays");

    h = gui.addFolder("Camera parameters");
    h.add(controller, "cx", -50.0, 50.0, 0.0).name("position.x");
    h.add(controller, "cy", -50.0, 50.0, 0.0).name("position.y");
    h.add(controller, "cz", -50.0, 100.0, 35.0).name("position.z");
    h.add(controller, "tx", -50.0, 50.0, 0.0).name("target.x");
    h.add(controller, "ty", -50.0, 50.0, 0.0).name("target.y");
    h.add(controller, "tz", -50.0, 50.0, 35.0).name("target.z");
    h.add(controller, "upx", -1.0, 1.0, 0.0).name("up.x");
    h.add(controller, "upy", -1.0, 1.0, 1.0).name("up.y");
    h.add(controller, "upz", -1.0, 1.0, 0.0).name("up.z");
    h.add(controller, "fov", 0.0, 180.0, 45.0).name("fov");
    h.add(controller, "near", -10.0, 50.0, 0.0).name("near");
    h.add(controller, "cameraCoords", false).name("coords");
    h.add(controller, "fustrum", false).name("fustrum");
    h.add(controller, "setCamera").name("setCamera");
    
    
    for(var i=0; i<scene.lights.length; i++)
    {
        if(scene.lights[i].__type__ == "point_light")
        { 
            controller["l" + i + "x"] = scene.lights[i].position[0];
            controller["l" + i + "y"] = scene.lights[i].position[1];
            controller["l" + i + "z"] = scene.lights[i].position[2];
            h = gui.addFolder("Light " + i + " parameters");
            h.add(controller, "l" + i + "x", -50.0, 50.0, 0.0).name("light.x");
            h.add(controller, "l" + i + "y", -50.0, 50.0, 10.0).name("light.y");
            h.add(controller, "l" + i + "z", -50.0, 50.0, 10.0).name("light.z");
        }
    }

    for(var i=0; i<scene.objects.length; i++)
    {
        if(scene.objects[i].__type__ == "sphere")
        { 
            controller["s" + i + "x"] = scene.objects[i].position[0];
            controller["s" + i + "y"] = scene.objects[i].position[1];
            controller["s" + i + "z"] = scene.objects[i].position[2];
            controller["radius" + i] = scene.objects[i].radius;
            h = gui.addFolder("Object " + i + " parameters");
            h.add(controller, "s" + i + "x", -50.0, 50.0, 0.0).name("sphere.x");
            h.add(controller, "s" + i + "y", -50.0, 50.0, 0.0).name("sphere.y");
            h.add(controller, "s" + i + "z", -50.0, 50.0, 0.0).name("sphere.z");
            h.add(controller, "radius" + i, 0.0, 50.0, 5.0).name("radius");
        }
    }

}

//

function animate() {

    requestAnimationFrame(animate);
    render();

}

function render() {

    var delta = clock.getDelta();
    cameraControls.update(delta);

    for(var i=0; i<scene.image.width; i++)
	{
		for(var j=0; j<scene.image.height; j++)
		{
            var index = i + scene.image.width*j;
			var ray = generatePixelRay(scene.camera, i, j, scene.image.width, scene.image.height);
            updateArrow(scene.image.visualObject.rays[index], ray.position, ray.direction, 0.02, 8000, controller.rays);
        }
    }
    
    updateArrow(scene.visualObject.coords[0], [0,0,0], [1,0,0], 0.07, 2, controller.worldCoords);
    updateArrow(scene.visualObject.coords[1], [0,0,0], [0,1,0], 0.07, 2, controller.worldCoords);
    updateArrow(scene.visualObject.coords[2], [0,0,0], [0,0,1], 0.07, 2, controller.worldCoords);
    
    scene.camera.target = [controller.tx, controller.ty, controller.tz];
    scene.camera.visualObject.target.position.set(controller.tx, controller.ty, controller.tz);
    
    scene.camera.position = [controller.cx, controller.cy, controller.cz];
    scene.camera.near = controller.near;
    scene.camera.fov = controller.fov;
    scene.camera.up = [controller.upx, controller.upy, controller.upz];
    scene.camera.up = normalize(scene.camera.up);
    setCameraBounds(scene.camera, scene.image.width, scene.image.height);
    setCameraCoordinatesBasis(scene.camera);
    
    var cameraBoxPosition = add(scene.camera.position, mult_scalar(2, scene.camera.coordinateBasis[2]));
    scene.camera.visualObject.obj.position.set(cameraBoxPosition[0], cameraBoxPosition[1], cameraBoxPosition[2]);
    updateObjectOrientationUsingBasis(scene.camera.visualObject.obj, scene.camera.coordinateBasis);
    //updateObjectOrientation(scene.camera.visualObject.obj, scene.camera.coordinateBasis[2], [0,1,0]);
    
    var upPosition = add(cameraBoxPosition, mult_scalar(1, scene.camera.coordinateBasis[1]));
    updateArrow(scene.camera.visualObject.up, upPosition, scene.camera.up, 0.07, 2, true);

    for(var i=0; i<3; i++)
    {

        updateArrow(scene.camera.visualObject.coords[i], scene.camera.position, scene.camera.coordinateBasis[i], 0.07, 2, controller.cameraCoords);
    }
    
    var fustrumCorners = getFustrumCorners(scene.camera);
    for(var i=0; i<4; i++)
    {
        var dir = normalize(sub(fustrumCorners[i], scene.camera.position));
        updateArrow(scene.camera.visualObject.fustrum[i], scene.camera.position, dir, 0.01, 8000, controller.fustrum);
    }
    
    var imageCoords = cameraToWorldCoords(scene.camera, [0,0,-controller.near]);
    scene.image.visualObject.obj.position.set(imageCoords[0], imageCoords[1], imageCoords[2]);
    scene.image.visualObject.obj.scale.x = scene.camera.cameraBounds.r - scene.camera.cameraBounds.l;     
    scene.image.visualObject.obj.scale.y = scene.camera.cameraBounds.t - scene.camera.cameraBounds.b;     
    //updateObjectOrientation(scene.image.visualObject.obj, scene.camera.coordinateBasis[1], [0,1,0]);
    updateObjectOrientationUsingBasis(scene.image.visualObject.obj, scene.camera.coordinateBasis);

    var imageOrigin = cameraToWorldCoords(scene.camera, pixelToCameraCoords(scene.camera, 0, 0, scene.image.width, scene.image.height));    
    updateArrow(scene.image.visualObject.coords[0], imageOrigin, scene.camera.coordinateBasis[0], 0.03, 1, controller.imageCoords);
    updateArrow(scene.image.visualObject.coords[1], imageOrigin, scene.camera.coordinateBasis[1], 0.03, 1, controller.imageCoords);
    
    for(var i=0; i<scene.lights.length; i++)
    {
        if(scene.lights[i].__type__ == "point_light")
        { 
            scene.lights[i].visualObject.obj.position.set(controller["l" + i + "x"], 
                                                          controller["l" + i + "y"], 
                                                          controller["l" + i + "z"]);
            scene.lights[i].visualObject.light3js.position.set(controller["l" + i + "x"], 
                                                          controller["l" + i + "y"], 
                                                          controller["l" + i + "z"]);
        }
    }
    for(var i=0; i<scene.objects.length; i++)
    {
        if(scene.objects[i].__type__ == "sphere")
        { 
            scene.objects[i].visualObject.obj.position.set(controller["s" + i + "x"], 
                                                          controller["s" + i + "y"], 
                                                          controller["s" + i + "z"]);
            scene.objects[i].visualObject.obj.scale.set(controller["radius" + i], 
                                                       controller["radius" + i], 
                                                       controller["radius" + i]);
        }
    }

    renderer.render(scene3js, camera3js);

}

function fillScene() {
    scene3js = new THREE.Scene();
    scene3js.fog = new THREE.Fog( 0x808080, 2000, 4000 );
    scene3js.add(camera3js);
    
    var ground = new THREE.Mesh(
        new THREE.PlaneGeometry( 1000, 1000, 100, 100 ),
        new THREE.MeshBasicMaterial( { color: 0xCCCCCC, wireframe: true } ) );
    ground.rotation.x = - Math.PI / 2;
    ground.position.y = -15;
    scene3js.add( ground );
    
    for(var i=0; i<scene.objects.length; i++)
    {
        if(scene.objects[i].__type__ == "sphere")
        {
            scene3js.add( scene.objects[i].visualObject.obj );
        }
    }

    for(var i=0; i<scene.lights.length; i++)
    {
        if(scene.lights[i].__type__ == "point_light")
        {
            scene3js.add(scene.lights[i].visualObject.obj );
            scene3js.add(scene.lights[i].visualObject.light3js);
        }

    }

    scene3js.add( scene.camera.visualObject.obj );
    scene3js.add( scene.camera.visualObject.target );
    scene3js.add( scene.camera.visualObject.up );
    for(var i=0; i<3; i++)
    {
        scene3js.add(scene.camera.visualObject.coords[i]);
    }
    for(var i=0; i<4; i++)
    {
        scene3js.add(scene.camera.visualObject.fustrum[i]);
    }

    scene3js.add( scene.image.visualObject.obj );
    for(var i=0; i<2; i++)
    {
        scene3js.add(scene.image.visualObject.coords[i]);
    }
    for(var i=0; i<scene.image.visualObject.rays.length; i++)
    {
        scene3js.add(scene.image.visualObject.rays[i]);
    }

    for(var i=0; i<3; i++)
    {
        scene3js.add(scene.visualObject.coords[i]);
    }
}