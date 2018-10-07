// Onload function.
window.onload = function () {

    // Init function.
    function init() {
        /**
         ** Camera settings.
         **/
        camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 1, 1500);
        cameraControls = new THREE.OrbitControls(camera);
        camera.position.z = -5;
        camera.position.y = 6;
        camera.position.x = 50;
        cameraControls.minDistance = 10;
        cameraControls.maxDistance = 50;
        cameraControls.maxPolarAngle = Math.PI / 2;
        cameraControls.maxZoom = 10;
        cameraControls.update();

        /**
         ** Scene settings.
         **/
        scene = new THREE.Scene();
        fogColor = new THREE.Color(0xf1f1f1);
        scene.background = fogColor;
        scene.fog = new THREE.Fog(fogColor, 0.015, 300);

        /**
         ** Light settings.
         **/
        var light = new THREE.SpotLight(0xffffff, 0.05);
        light.position.set(-50, 100, 100);
        light.castShadow = true;
        light.shadowCameraVisible = true;

        // Set up shadow properties for the light.
        light.shadow.mapSize.width = 512;
        light.shadow.mapSize.height = 512;
        light.shadow.camera.near = 50;
        light.shadow.camera.far = 300;

        // add lights to scene.
        scene.add(light);
        scene.add(light.target);

        /**
         ** Renderer settings.
         **/
        renderer = new THREE.WebGLRenderer({ antialias: true });
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.shadowMap.enabled = true;
        renderer.shadowMap.type = THREE.PCFSoftShadowMap
        renderer.setSize(window.innerWidth, window.innerHeight);
        renderer.setClearColor(0x00000, 1.0);
        renderer.shadowMapEnabled = true;
        renderer.setSize(window.innerWidth, window.innerHeight + 5);

        // Append render settings to html document.
        document.body.appendChild(renderer.domElement);

        // Three.JS interaction function.
        var interaction = new THREE.Interaction(renderer, scene, camera);

        // Resize handler.
        window.addEventListener('resize', onWindowResize, false);

        // Three.JS handler.
        THREE.Loader.Handlers.add(/\.tga$/i, new THREE.TGALoader());

        // Camera box settings.
        function onPositionChange() {
            if (cameraControls.target.x > 100) {
                camera.position.x = 100;
                cameraControls.target.x = 100;
            }

            if (cameraControls.target.x < -100) {
                camera.position.x = -100;
                cameraControls.target.x = -100;
            }

            if (cameraControls.target.z > 100) {
                camera.position.z = 100;
                cameraControls.target.z = 100;
            }

            if (cameraControls.target.z < -100) {
                camera.position.z = -100;
                cameraControls.target.z = -100;
            }
        }

        // Add listener for camera movement.
        cameraControls.addEventListener('change', onPositionChange);

        /**
         ** Skybox.
         **/

        // cube.
        var cube = new THREE.CubeGeometry(1000, 1000, 1000);
        var cubeMaterials = [
            // Front side.
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_ft.tga'),
                side: THREE.DoubleSide
            }),
            // Back side.
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_bk.tga'),
                side: THREE.DoubleSide
            }),
            // Top side.
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_up.tga'),
                side: THREE.DoubleSide
            }),
            // Bottom side.
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_dn.tga'),
                side: THREE.DoubleSide
            }),
            // Right side.
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_rt.tga'),
                side: THREE.DoubleSide
            }),
            // Left side.
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_lf.tga'),
                side: THREE.DoubleSide
            })
        ];

        // Add cube & materials.
        var cubeMaterial = new THREE.MeshFaceMaterial(cubeMaterials);
        var mesh = new THREE.Mesh(cube, cubeMaterial);
        scene.add(mesh);

        // Ambient light.
        var AmbientLight = new THREE.AmbientLight(0x404040);
        AmbientLight.intensity = 4;
        scene.add(AmbientLight);

        THREE.Loader.Handlers.add(/\.dds$/i, new THREE.DDSLoader());

        /**
         ** Platform loading.
         **/
        Loading.OBJModel('obj/platform/', 'loading_platform.obj', 'obj/platform/', 'loading_platform.mtl', (mesh) => {
            mesh.scale.set(20, 20, 20);

            mesh.position.y = 0;
            mesh.position.z = 0;
            mesh.position.x = 0;

            cranemove.add(mesh.getObjectByName("CraneMoveBlock_1_mesh1169467915"));
            cranemove.add(mesh.getObjectByName("CraneMoveBlock_2_mesh1697543657"));
            cranemove.add(mesh.getObjectByName("CraneMoveBlock_3_mesh1358320812"));

            var crane = mesh.getObjectByName("Crane_mesh915592506");

            platformGroup.add(mesh);
            platform = mesh;
        });

        Loading.OBJModel('obj/platform/', 'platform_detail.obj', 'obj/platform/', 'platform_detail.mtl', (mesh) => {
            mesh.scale.set(20, 20, 20);

            mesh.position.y = 0;
            mesh.position.z = 0;
            mesh.position.x = 0;

            platformGroup.add(mesh);
        });

        /**
         ** Load dock loading.
         **/
        Loading.OBJModel('obj/laadruimte/', 'laadruimte.obj', 'obj/laadruimte/', 'laadruimte.mtl', (mesh) => {
            mesh.position.y = 6.2;
            mesh.position.z = 22.7;
            mesh.position.x = 3.23;

            mesh.rotation.y = 1.57;
            mesh.scale.set(10, 10, 10);

            doorRight.add(mesh.getObjectByName("DoorRight_factory.002"));
            doorLeft.add(mesh.getObjectByName("DoorLeft_factory.001"));

            warehouse.add(mesh);
        });


        /**
         ** Add to scenes.
         **/
        scene.add(warehouse);
        scene.add(platformGroup);
        scene.add(truckGroup);
        scene.add(boatGroup);
        scene.add(cranemove);
    }

    // Resize function.
    function onWindowResize() {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    // Animate function.
    function animate() {
        requestAnimationFrame(animate);
        cameraControls.update();
        renderer.render(scene, camera);
    }

    // Sockets
    exampleSocket = new WebSocket("ws://" + window.location.hostname + ":" + window.location.port + "/connect_client");
    exampleSocket.onmessage = function (event) {
        var command = parseCommand(event.data);
        
        if (command.command === "delete") {
            console.log("delete");
            if (!(Object.keys(worldObjects).indexOf(command.parameters.guid) < 0)) {
                var object = worldObjects[command.parameters.guid];
                
                switch (command.parameters.type) {
                    case "robot":
                        // Remove robot object.
                        robotsGroup.remove(object);
                        break;
                }

                // Remove object from world.
                delete worldObjects[command.parameters.guid];
            }
        }

        if (command.command === "update") {
            if (command.parameters.type === "robot") {
                var robot;

                if (Object.keys(worldObjects).indexOf(command.parameters.guid) < 0) {
                    // Create new robot.
                    robot = new Robot();
                    
                    worldObjects[command.parameters.guid] = robot;
                    // Add robot to group.
                    robotsGroup.add(robot);
                } else {
                    robot = worldObjects[command.parameters.guid];
                }
                    
                robot.position.x = command.parameters.x;
                robot.position.y = command.parameters.y;
                robot.position.z = command.parameters.z;
                robot.rotation.y = command.parameters.rotationY;
            }

            if (command.parameters.type === "rack") {

                var rack;

                if (Object.keys(worldObjects).indexOf(command.parameters.guid) < 0) {
                    // Create new rack.
                    rack = new Rack();

                    worldObjects[command.parameters.guid] = rack;
                    // Add rack to group.
                    racksGroup.add(rack);

                    // Fill rack with boxes.
                    command.parameters.Boxes.forEach(function (boxInfo) {
                        var box;
                        // Create a new box.
                        box = new Box();

                        box.position.set(boxInfo.x, boxInfo.y, boxInfo.z);
                        box.rotation.set(boxInfo.rotationX, boxInfo.rotationY, boxInfo.rotationZ);
                        box.scale.set(boxInfo.scaleX, boxInfo.scaleY, boxInfo.scaleZ);

                        worldObjects[boxInfo.guid] = box;
                        // Add box to rack.
                        rack.add(box);
                        // Add box to array.
                        boxes.push(box);
                    });

                } else {
                    rack = worldObjects[command.parameters.guid];
                }
                
                rack.scale.set(25, 25, 25);
                rack.position.x = command.parameters.x;
                rack.position.y = command.parameters.y;
                rack.position.z = command.parameters.z;
                rack.rotation.x = command.parameters.rotationX;
                rack.rotation.y = command.parameters.rotationY;
            }

            if (command.parameters.type === "truck") {
                var truck;

                if (Object.keys(worldObjects).indexOf(command.parameters.guid) < 0) {
                    truck = new Truck();

                    worldObjects[command.parameters.guid] = truck;
                    truckGroup.add(truck);
                } else {
                    truck = worldObjects[command.parameters.guid];
                }
                
                if (truck.position.z < 2 && truck.position.z >= 0.0005)
                    truck.breakLights();
                else
                    truck.normalLights();

                truck.position.x = command.parameters.x;
                truck.position.y = command.parameters.y;
                truck.position.z = command.parameters.z;
            }

            if (command.parameters.type === "boat") {
                var boat;

                if (Object.keys(worldObjects).indexOf(command.parameters.guid) < 0) {
                    boat = new Boat();

                    worldObjects[command.parameters.guid] = boat;
                    boatGroup.add(boat);
                } else {
                    boat = worldObjects[command.parameters.guid];
                }

                boat.position.x = command.parameters.x;
                boat.position.y = command.parameters.y;
                boat.position.z = command.parameters.z;
            }

            if (command.parameters.type === "doors") {
                var difference = command.parameters.x - doorRight.position.x;
                    
                doorRight.position.x += difference;
                doorLeft.position.x -= difference;
            }

            if (command.parameters.type === "crane") {
                if (command.parameters.vehicle === "truck" && command.parameters._CraneState === 1) {
                    if (Object.keys(worldObjects).indexOf(command.parameters.vehicleID) >= 0)
                        getTruckContainer(worldObjects[command.parameters.vehicleID]);
                }
                if (command.parameters.vehicle === "truck" && command.parameters._CraneState === 2) {
                    if (Object.keys(worldObjects).indexOf(command.parameters.vehicleID) >= 0)
                        removeTruckContainer(worldObjects[command.parameters.vehicleID]);
                }

                if (command.parameters.vehicle === "boat" && command.parameters._CraneState === 1) {
                    if (Object.keys(worldObjects).indexOf(command.parameters.vehicleID) >= 0)
                        getBoatContainer(worldObjects[command.parameters.vehicleID]);
                }

                if (command.parameters.vehicle === "boat" && command.parameters._CraneState === 2) {
                    if (Object.keys(worldObjects).indexOf(command.parameters.vehicleID) >= 0)
                        removeBoatContainer(worldObjects[command.parameters.vehicleID]);
                }
            }
        }
    }

    init();
    animate();
}