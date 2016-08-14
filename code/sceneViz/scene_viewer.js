/*global THREE, requestAnimationFrame, Detector, dat */
if (!Detector.webgl) { Detector.addGetWebGLMessage(); }

var SCREEN_WIDTH = window.innerWidth;
var SCREEN_HEIGHT = window.innerHeight;

var container, camera3js, scene3js, renderer;

var cameraControls;
var controller;
var clock = new THREE.Clock();

var image = {"width": 32, "height": 32};

init();
animate();

function init3jsObjects()
{
    var cameraMaterial = new THREE.MeshBasicMaterial( { color: 0x070707, wireframe: true  } );
    var boxObj = new THREE.Mesh(new THREE.CubeGeometry( 2, 2, 4 ), cameraMaterial );
    var targetObj = new THREE.Mesh(
        new THREE.SphereGeometry( 0.5, 8, 4 ), cameraMaterial );
    var imageObj = new THREE.Mesh(
        new THREE.PlaneGeometry( 10, 10, image.width, image.height ),
        new THREE.MeshBasicMaterial( { color: 0x0, wireframe: true } ) );
    var imageCoords = cameraToWorldCoords(scene.camera, [0,0,-scene.camera.near]);
    imageObj.position.set(imageCoords[0], imageCoords[1], imageCoords[2]);    
    scene.camera.visualObject =  {"material": cameraMaterial, "obj": boxObj, "target": targetObj, "image": imageObj};
    
    for(var i=0; i<scene.lights.length; i++)
    {
        var lightMaterial = new THREE.MeshLambertMaterial( { emissive: 0xFFFF00 } );
        var lightObj = new THREE.Mesh(new THREE.SphereGeometry( 1, 32, 16 ), lightMaterial );
        var light3js = new THREE.PointLight(0xffffff, 1.0);    
        scene.lights[i].visualObject = {"material": lightMaterial, "obj": lightObj, "light3js": light3js};
    }

    for(var i=0; i<scene.objects.length; i++)
    {
        var sphereMaterial = new THREE.MeshLambertMaterial( { color: 0xA00000 } );
        var sphere = new THREE.Mesh(new THREE.SphereGeometry( 1, 32, 16 ), sphereMaterial );
        scene.objects[i].visualObject = {"material": sphereMaterial, "obj": sphere};
    }
}

function init() {

    loadResources(resources);
	loadScene(scene);

    init3jsObjects();

    container = document.createElement('div');
    document.body.appendChild(container);

    // CAMERA
    camera3js = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 80000);
    camera3js.position.set(-100, 150, 100);

    // LIGHTS
    //light = new THREE.PointLight(0xffffff, 1.0);
    //light.position.set(0, 10, 10);

    // RENDERER
    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
    renderer.setClearColorHex(0xAAAAAA, 1.0);
    container.appendChild(renderer.domElement);

    renderer.gammaInput = true;
    renderer.gammaOutput = true;

    // EVENTS
    window.addEventListener('resize', onWindowResize, false);
    document.addEventListener('keydown', onKeyDown, false);
    document.addEventListener('keyup', onKeyUp, false);

    // CONTROLS
    cameraControls = new THREE.TrackballControls(camera3js, renderer.domElement);
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
    h.add(controller, "near", -10.0, 50.0, 1.0).name("near");
    
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

    scene.camera.visualObject.obj.position.set(controller.cx, controller.cy, controller.cz);
    scene.camera.visualObject.target.position.set(controller.tx, controller.ty, controller.tz);
    var imageCoords = cameraToWorldCoords(scene.camera, [0,0,-controller.near]);
    scene.camera.visualObject.image.position.set(imageCoords[0], imageCoords[1], imageCoords[2]);    
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
        new THREE.MeshBasicMaterial( { color: 0x0, wireframe: true } ) );
    ground.rotation.x = - Math.PI / 2;
    ground.position.y = -10;
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
    scene3js.add( scene.camera.visualObject.image );
}