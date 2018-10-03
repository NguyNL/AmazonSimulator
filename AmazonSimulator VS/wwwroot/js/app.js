function parseCommand(input = "") {
    return JSON.parse(input);
}

var exampleSocket;
var boxes = [];
window.onload = function () {

    /**
     ** VARIABLES
     **/

    // CAMERA AND RENDERING
    var camera, scene, renderer, cameraControls,
        worldObjects = {},

        // Skybox Map & Name
        textMap = 'ely_hills',
        textName = 'hills',

        // Crane variables
        boatLoadXPosition = -4,
        boatX = 2.15,
        boatZ = -2.9,

        TruckLoadXPosition = 21,
        truckX = 0,
        truckZ = -26,

        // Loading and unloading
        animationInProgress = false,
        loadDeckIsLoadedWith = false,
        loadProcess = 0,
        unloadProcess = 0;

        // Doors loadingdeck
        //doorRight,
        //doorLeft;

    /**
     ** GROUPS
     **/
    var platformGroup = new THREE.Group();

    // Truck
    var truck = new THREE.Group();
    truck.scale.set(20, 20, 20);
    truck.position.x = truckX;
    truck.position.z = 0;

    // Boat
    var boat = new THREE.Group();
    var boatContainer = new THREE.Group();
    boatContainer.position.x = -1.16;
    boatContainer.position.z = -0.632;
    boatContainer.position.y = 2.4;

    boat.scale.set(20, 20, 20);
    boat.position.x = boatX;
    boat.position.z = 16;//boatZ;

    // CraneMove
    var cranemove = new THREE.Group();
    cranemove.scale.set(20, 20, 20);
    cranemove.position.x = 35;

    // Warehouse
    var warehouse = new THREE.Group();
    warehouse.position.x = 17;
    warehouse.position.y = -4.39;
    warehouse.position.z = -30;

    // warehouseGrid
    var warehouseGrid = new THREE.Group();
    warehouseGrid.rotation.y = 90 * Math.PI / 180;
    warehouseGrid.position.x = 3.8;
    warehouseGrid.position.z = 32;
    warehouse.add(warehouseGrid);

    // Robot
    var robotsGroup = new THREE.Group();
    robotsGroup.position.y = 0.2;
    robotsGroup.scale.set(0.06, 0.06, 0.06);
    warehouseGrid.add(robotsGroup);

    // Rack
    var racksGroup = new THREE.Group();
    racksGroup.position.y = 1.57;
    racksGroup.scale.set(0.06, 0.06, 0.06);
    warehouseGrid.add(racksGroup);

    // Doors
    var doorLeft = new THREE.Group();
    var doorRight = new THREE.Group();

    doorRight.position.z = -1.48;
    doorLeft.position.z = -1.48;

    doorRight.position.y = 7;
    doorLeft.position.y = 7;

    doorRight.position.x = 9.856;
    doorLeft.position.x = 9.856;

    doorRight.scale.set(12, 12, 12);
    doorLeft.scale.set(12, 12, 12);

    warehouseGrid.add(doorLeft);
    warehouseGrid.add(doorRight);

    function init() {
        /**
         ** CAMERA & RENDER SETTINGS
         **/
        camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 1, 1500);
        cameraControls = new THREE.OrbitControls(camera);
        camera.position.z = 15;
        camera.position.y = 5;
        camera.position.x = 15;
        cameraControls.update();
        scene = new THREE.Scene();

        renderer = new THREE.WebGLRenderer({ antialias: true });
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(window.innerWidth, window.innerHeight + 5);
        document.body.appendChild(renderer.domElement);

        var interaction = new THREE.Interaction(renderer, scene, camera);

        window.addEventListener('resize', onWindowResize, false);

        THREE.Loader.Handlers.add(/\.tga$/i, new THREE.TGALoader());

        /**
         ** SKYBOX
         **/

        // cube
        var cube = new THREE.CubeGeometry(1000, 1000, 1000);
        var cubeMaterials = [
            // front side
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_ft.tga'),
                side: THREE.DoubleSide
            }),
            // back side
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_bk.tga'),
                side: THREE.DoubleSide
            }),
            // Top side
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_up.tga'),
                side: THREE.DoubleSide
            }),
            // Bottom side
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_dn.tga'),
                side: THREE.DoubleSide
            }),
            // right side
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_rt.tga'),
                side: THREE.DoubleSide
            }),
            // left side
            new THREE.MeshBasicMaterial({
                map: new THREE.TGALoader().load('textures/skybox/' + textMap + '/' + textName + '_lf.tga'),
                side: THREE.DoubleSide
            })
        ];

        //add cube & materials
        var cubeMaterial = new THREE.MeshFaceMaterial(cubeMaterials);
        var mesh = new THREE.Mesh(cube, cubeMaterial);
        scene.add(mesh);

        /**
         ** LIGHT
         **/

        var light = new THREE.AmbientLight(0x404040);
        light.intensity = 4;
        scene.add(light);

        THREE.Loader.Handlers.add(/\.dds$/i, new THREE.DDSLoader());

        function unloadLoadingDeck() {
            switch (loadDeckIsLoadedWith) {
                case "boat":
                    removeBoatContainer();
                    break;
                case "truck":
                    removeTruckContainer();
                    break;
            }
        }

        var allProcesses = [];
        var allProcessesIndex = 0;
        function loadProgressUpdate(progress, processesCount, loading = true) {
            if (allProcesses.length === 0) {
                allProcesses.push(progress);

                if (loading) unloadProcess = 0;
            }
            else
                allProcesses[allProcessesIndex] = progress;

            if (progress === 1) {
                allProcesses.push(0);
                allProcessesIndex++;
            }

            if (loading)
                loadProcess = allProcesses.reduce(function (a, b) { return a + b; }, 0) / processesCount;
            else
                unloadProcess = allProcesses.reduce(function (a, b) { return a + b; }, 0) / processesCount;


            console.log(
                loadDeckIsLoadedWith + ": loading: " + (Math.round(loadProcess * 10000) / 100).toString().replace(".", ",") + "% - " +
                "unloading: " + (Math.round(unloadProcess * 10000) / 100).toString().replace(".", ",") + "% - " +
                "Total loading: " + (Math.round(((unloadProcess + loadProcess) / 2) * 10000) / 100).toString().replace(".", ",") + "%"
            );
        }

        function removeTruckContainer() {
            if (animationInProgress) return;
            if (!loadDeckIsLoadedWith) return;

            animationInProgress = true;

            TweenMax.to(truck.children[1].position, 2,
                {
                    z: 0,
                    onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
                    onComplete: function () {
                        TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });
                        TweenMax.to([
                            cranemove.children[1].position,
                            cranemove.children[2].position], 2, { y: 0 });
                        TweenMax.to(truck.children[1].position, 2,
                            {
                                y: 0.374,
                                onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
                                onComplete: function () {

                                    TweenMax.to([
                                        cranemove.children[0].position,
                                        cranemove.children[1].position,
                                        cranemove.children[2].position,
                                        truck.children[1].position], 3,
                                        {
                                            x: 0,
                                            onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
                                            onComplete: function () {
                                                TweenMax.to(cranemove.children[1].scale, 2, { y: 2 });
                                                TweenMax.to(cranemove.children[1].position, 2, { y: -0.831 });
                                                TweenMax.to(cranemove.children[2].position, 2, { y: -0.384 });

                                                TweenMax.to(truck.children[1].position, 2,
                                                    {
                                                        y: -0.01,
                                                        onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
                                                        onComplete: function () {
                                                            TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });

                                                            TweenMax.to([
                                                                cranemove.children[2].position,
                                                                cranemove.children[1].position], 2,
                                                                {
                                                                    y: 0,
                                                                    onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
                                                                    onComplete: function () {
                                                                        animationInProgress = false;
                                                                        loadDeckIsLoadedWith = false;
                                                                        allProcesses = [];
                                                                        allProcessesIndex = 0;
                                                                    }

                                                                });
                                                        }
                                                    });
                                            }
                                        });
                                }
                            });
                    }
                });
        }

        function getTruckContainer() {
            if (animationInProgress) return;
            if (loadDeckIsLoadedWith) return unloadLoadingDeck();

            animationInProgress = true;
            loadDeckIsLoadedWith = "truck";

            var distanceAnimationRatio = 0.1;
            var animationTimeX = Math.abs(truck.position.x - truckX) * distanceAnimationRatio;
            var animationTimeZ = Math.abs(truck.position.z - truckZ) * distanceAnimationRatio;

            if (truck.position.x != truckX)
                TweenMax.to(truck.position, animationTimeX, { x: truckX });

            if (truck.position.z != truckZ)
                TweenMax.to(truck.position, animationTimeZ, { z: truckZ, });

            var AnimationDelay = animationTimeX > animationTimeZ ? animationTimeX : animationTimeZ;

            TweenMax.to(cranemove.position, Math.abs(TruckLoadXPosition - cranemove.position.x) * distanceAnimationRatio, {
                x: TruckLoadXPosition,
                delay: AnimationDelay,
                onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6); },
                onComplete: function () {
                    TweenMax.to(cranemove.children[1].scale, 2, { y: 2 });
                    TweenMax.to(cranemove.children[1].position, 2, { y: -0.831 });

                    TweenMax.to(cranemove.children[2].position, 2,
                        {
                            y: -0.384,
                            onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                            onComplete: function () {
                                TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });
                                TweenMax.to([cranemove.children[1].position, cranemove.children[2].position], 2, { y: 0 });

                                TweenMax.to(truck.children[1].position, 2,
                                    {
                                        y: 0.374,
                                        onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                                        onComplete: function () {
                                            TweenMax.to([
                                                cranemove.children[0].position,
                                                cranemove.children[1].position,
                                                cranemove.children[2].position,
                                                truck.children[1].position], 3,
                                                {
                                                    x: 0.855,
                                                    onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                                                    onComplete: function () {
                                                        TweenMax.to(cranemove.children[1].scale, 2, { y: 1.95 });
                                                        TweenMax.to(cranemove.children[1].position, 2, { y: -0.8 });
                                                        TweenMax.to(cranemove.children[2].position, 2, { y: -0.368 });
                                                        TweenMax.to(truck.children[1].position, 2,
                                                            {
                                                                y: 0.006,
                                                                onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                                                                onComplete: function () {
                                                                    TweenMax.to(truck.children[1].position, 2,
                                                                        {
                                                                            z: 0.2,
                                                                            onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                                                                            onComplete: function () {
                                                                                animationInProgress = false;
                                                                                allProcesses = [];
                                                                                allProcessesIndex = 0;
                                                                            }
                                                                        });
                                                                },
                                                            });
                                                    }
                                                });
                                        }
                                    });
                            },
                        });
                },
            });
        }

        function removeBoatContainer() {
            if (animationInProgress) return;
            if (!loadDeckIsLoadedWith) return;

            animationInProgress = true;

            TweenMax.to(boatContainer.position, 2,
                {
                    z: -0.632,
                    onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                    onComplete: function () {
                        TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });
                        TweenMax.to([
                            cranemove.children[1].position,
                            cranemove.children[2].position,
                        ], 2, { y: 0 });

                        TweenMax.to(boatContainer.position, 2,
                            {
                                y: 2.96,
                                onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                                onComplete: function () {

                                    TweenMax.to(boatContainer.rotation, 2,
                                        {
                                            y: 0,
                                            onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                                            onComplete: function () {
                                                TweenMax.to([
                                                    cranemove.children[0].position,
                                                    cranemove.children[1].position,
                                                    cranemove.children[2].position], 4, { x: 0 });

                                                TweenMax.to(
                                                    boatContainer.position, 4,
                                                    {
                                                        x: -1.16,
                                                        onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                                                        onComplete: function () {
                                                            TweenMax.to(cranemove.children[1].scale, 2, { y: 2.6 });
                                                            TweenMax.to(cranemove.children[1].position, 2, { y: -1.3 });
                                                            TweenMax.to(cranemove.children[2].position, 2, { y: -0.56 });

                                                            TweenMax.to(boatContainer.position, 2,
                                                                {
                                                                    y: 2.4,
                                                                    onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                                                                    onComplete: function () {
                                                                        TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });
                                                                        TweenMax.to([
                                                                            cranemove.children[1].position,
                                                                            cranemove.children[2].position,
                                                                        ], 2, {
                                                                                y: 0,
                                                                                onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                                                                                onComplete: function () {
                                                                                    animationInProgress = false;
                                                                                    loadDeckIsLoadedWith = false;

                                                                                    allProcesses = [];
                                                                                    allProcessesIndex = 0;
                                                                                }
                                                                            });
                                                                    }
                                                                });
                                                        }
                                                    });
                                            }
                                        });
                                }
                            })
                    }
                });

        }

        function getBoatContainer() {
            if (animationInProgress) return;
            if (loadDeckIsLoadedWith) return unloadLoadingDeck();

            animationInProgress = true;
            loadDeckIsLoadedWith = "boat";

            var distanceAnimationRatio = 0.1;
            var animationTimeX = Math.abs(boat.position.x - boatX) * distanceAnimationRatio;
            var animationTimeZ = Math.abs(boat.position.z - boatZ) * distanceAnimationRatio;

            if (boat.position.x != boatX)
                TweenMax.to(boat.position, animationTimeX, { x: boatX });

            if (boat.position.z != boatZ)
                TweenMax.to(boat.position, animationTimeZ, { z: boatZ, });

            var AnimationDelay = animationTimeX > animationTimeZ ? animationTimeX : animationTimeZ;

            TweenMax.to(cranemove.position, Math.abs(boatLoadXPosition - cranemove.position.x) * distanceAnimationRatio, {
                x: boatLoadXPosition,
                delay: AnimationDelay,
                onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 7); },
                onComplete: function () {
                    TweenMax.to(cranemove.children[1].scale, 2, { y: 2.6 });
                    TweenMax.to(cranemove.children[1].position, 2, { y: -1.3 });

                    TweenMax.to(cranemove.children[2].position, 2,
                        {
                            y: -0.56,
                            onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                            onComplete: function () {
                                TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });
                                TweenMax.to([cranemove.children[1].position, cranemove.children[2].position], 2, { y: 0 });

                                TweenMax.to(boatContainer.position, 2,
                                    {

                                        y: 2.96,
                                        onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                        onComplete: function () {
                                            TweenMax.to([
                                                cranemove.children[0].position,
                                                cranemove.children[1].position,
                                                cranemove.children[2].position], 4, { x: 2.1 });

                                            TweenMax.to(
                                                boatContainer.position, 4,
                                                {
                                                    x: 0.94,
                                                    onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                                    onComplete: function () {
                                                        TweenMax.to([
                                                            boatContainer.rotation
                                                        ], 2, {
                                                                y: -90 * Math.PI / 180,
                                                                onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                                                onComplete: function () {
                                                                    TweenMax.to(cranemove.children[1].scale, 2, { y: 1.9 });
                                                                    TweenMax.to(cranemove.children[1].position, 2, { y: -0.765 });
                                                                    TweenMax.to(cranemove.children[2].position, 2, { y: -0.365 });
                                                                    TweenMax.to(boatContainer.position, 2,
                                                                        {
                                                                            y: 2.595,
                                                                            onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                                                            onComplete: function () {
                                                                                TweenMax.to(boatContainer.position, 2,
                                                                                    {
                                                                                        z: -0.4,
                                                                                        onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                                                                        onComplete: function () {
                                                                                            animationInProgress = false;
                                                                                            allProcesses = [];
                                                                                            allProcessesIndex = 0;
                                                                                        }
                                                                                    });
                                                                            },
                                                                        });
                                                                },
                                                            });
                                                    }
                                                });
                                        }
                                    });
                            },
                        });
                },
            });
        }

        /**
         ** PLATFORM(WORLD) LOADING
         **/
        Loading.OBJModel('obj/platform/', 'platform.obj', 'obj/platform/', 'platform.mtl', (mesh) => {
            mesh.scale.set(20, 20, 20);

            mesh.position.y = 0;
            mesh.position.z = 0;
            mesh.position.x = 0;

            cranemove.add(mesh.getObjectByName("CraneMoveBlock_1_mesh1169467915"));
            cranemove.add(mesh.getObjectByName("CraneMoveBlock_2_mesh1697543657"));
            cranemove.add(mesh.getObjectByName("CraneMoveBlock_3_mesh1358320812"));

            var crane = mesh.getObjectByName("Crane_mesh915592506");

            truck.add(mesh.getObjectByName("TruckLongOrange.001_mesh1355834799.001"));
            truck.add(mesh.getObjectByName("ContainerbigTruck_mesh1355834799.003"));

            var boatContainerObj = mesh.getObjectByName("containerBoat_mesh1355834799.000");
            boat.add(mesh.getObjectByName("Boat_mesh1196106066"));
            boatContainer.add(boatContainerObj);
            boatContainerObj.position.set(1.16, - 2.4, 0.632);
            boat.add(boatContainer);

            boat.cursor = 'pointer';
            truck.cursor = 'pointer';
            truck.children[1].position.y = -0.01;

            truck.on('mousedown', function (ev) {
                getTruckContainer();
            });

            boat.on('mousedown', function (ev) {
                getBoatContainer();
            });

            platformGroup.add(mesh);
            platform = mesh;
        });

        /**
         ** LOADDOCK LOADING
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
         ** ADD TO SCENE
         **/
        scene.add(warehouse);
        scene.add(platformGroup);
        scene.add(truck);
        scene.add(boat);
        scene.add(cranemove);
    }


    function onWindowResize() {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    function animate() {
        requestAnimationFrame(animate);
        cameraControls.update();
        renderer.render(scene, camera);
    }
    
    exampleSocket = new WebSocket("ws://" + window.location.hostname + ":" + window.location.port + "/connect_client");
    exampleSocket.onmessage = function (event) {
        var command = parseCommand(event.data);

        if (command.command === "update") {
            if (command.parameters.type === "robot") {
                var robot;

                if (Object.keys(worldObjects).indexOf(command.parameters.guid) < 0) {
                    robot = new Robot();
                    
                    worldObjects[command.parameters.guid] = robot;
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
                    rack = new Rack();

                    worldObjects[command.parameters.guid] = rack;
                    racksGroup.add(rack);

                    command.parameters.Boxes.forEach(function (boxInfo) {
                        var box;
                        
                        box = new Box();

                        box.position.set(boxInfo.x, boxInfo.y, boxInfo.z);
                        box.rotation.set(boxInfo.rotationX, boxInfo.rotationY, boxInfo.rotationZ);
                        box.scale.set(boxInfo.scaleX, boxInfo.scaleY, boxInfo.scaleZ);

                        worldObjects[boxInfo.guid] = box;
                        rack.add(box);
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

            if (command.parameters.type === "doors") {
                var difference = command.parameters.x - doorRight.position.x;
                    
                doorRight.position.x += difference;
                doorLeft.position.x -= difference;
            }
        }
    }

    init();
    animate();
}