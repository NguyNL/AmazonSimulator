﻿function parseCommand(input = "") {
    return JSON.parse(input);
}

// Add Pointlights.
function addPointLight(object, color, x, y, z, intensity, distance, decay, name = "") {
    var pointLight = new THREE.PointLight(color, intensity, distance);

    pointLight.position.set(x, y, z);
    pointLight.castShadow = true;

    pointLight.decay = decay;
    pointLight.name = name;

    object.add(pointLight);
}

// Add spotlights.
function addSpotLight(object, color, x, y, z, tx, ty, tz, decay, intensity, distance) {
    var spotLight = new THREE.SpotLight(color, intensity, distance);

    spotLight.position.set(x, y, z);
    spotLight.target.position.set(tx, ty, tz);

    spotLight.decay = decay;
    spotLight.castShadow = true;

    object.add(spotLight);
    object.add(spotLight.target);
}

// Add lensflares.
function Lensflare(object, texture, color, x, y, z, size) {

    var lensflare = new THREE.Lensflare();
    lensflare.addElement(
        new THREE.LensflareElement(
            texture,
            size, 0,
            new THREE.Color(color)
        )
    );
    lensflare.position.set(x, y, z);
    object.add(lensflare);
}

// Log progress in web console.
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


    //console.log(
    //    loadDeckIsLoadedWith + ": loading: " + (Math.round(loadProcess * 10000) / 100).toString().replace(".", ",") + "% - " +
    //    "unloading: " + (Math.round(unloadProcess * 10000) / 100).toString().replace(".", ",") + "% - " +
    //    "Total loading: " + (Math.round(((unloadProcess + loadProcess) / 2) * 10000) / 100).toString().replace(".", ",") + "%"
    //);
}

// Put truck container back on truck.
function removeTruckContainer(truck) {
    animationInProgress = true;

    var container = truck.getObjectByName("truck");
    container = container.getObjectByName("ContainerbigTruck_mesh1355834799.005");

    TweenMax.to(container.position, 2,
        {
            z: 0,
            onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
            onComplete: function () {
                TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });
                TweenMax.to([
                    cranemove.children[1].position,
                    cranemove.children[2].position], 2, { y: 0 });
                TweenMax.to(container.position, 2,
                    {
                        y: 0.384,
                        onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
                        onComplete: function () {

                            TweenMax.to([
                                cranemove.children[0].position,
                                cranemove.children[1].position,
                                cranemove.children[2].position,
                                container.position], 3,
                                {
                                    x: 0,
                                    onUpdate: function () { loadProgressUpdate(this.progress(), 5, false); },
                                    onComplete: function () {
                                        TweenMax.to(cranemove.children[1].scale, 2, { y: 2 });
                                        TweenMax.to(cranemove.children[1].position, 2, { y: -0.831 });
                                        TweenMax.to(cranemove.children[2].position, 2, { y: -0.384 });

                                        TweenMax.to(container.position, 2,
                                            {
                                                y: 0,
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

// Get truck container with crane.
function getTruckContainer(truck) {
    animationInProgress = true;
    loadDeckIsLoadedWith = "truck";

    TweenMax.to(cranemove.position, 2, {
        x: truckX,
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

                        var container = truck.getObjectByName("truck");
                        container = container.getObjectByName("ContainerbigTruck_mesh1355834799.005");
                        TweenMax.to(container.position, 2,
                            {
                                y: 0.384,
                                onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                                onComplete: function () {
                                    TweenMax.to([
                                        cranemove.children[0].position,
                                        cranemove.children[1].position,
                                        cranemove.children[2].position,
                                        container.position], 3,
                                        {
                                            x: 0.855,
                                            onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                                            onComplete: function () {
                                                TweenMax.to(cranemove.children[1].scale, 2, { y: 1.95 });
                                                TweenMax.to(cranemove.children[1].position, 2, { y: -0.8 });
                                                TweenMax.to(cranemove.children[2].position, 2, { y: -0.368 });
                                                TweenMax.to(container.position, 2,
                                                    {
                                                        y: 0.016,
                                                        onUpdate: function () { loadProgressUpdate(this.progress(), 6); },
                                                        onComplete: function () {
                                                            TweenMax.to(container.position, 2,
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

// Put boat container back on boat.
function removeBoatContainer(boat) {
    var container = boat.getObjectByName("boatContainer");

    animationInProgress = true;

    TweenMax.to(container.position, 2,
        {
            z: -0.632,
            onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
            onComplete: function () {
                TweenMax.to(cranemove.children[1].scale, 2, { y: 1 });
                TweenMax.to([
                    cranemove.children[1].position,
                    cranemove.children[2].position,
                ], 2, { y: 0 });

                TweenMax.to(container.position, 2,
                    {
                        y: 0.56,
                        onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                        onComplete: function () {

                            TweenMax.to(container.rotation, 2,
                                {
                                    y: 0,
                                    onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                                    onComplete: function () {
                                        TweenMax.to([
                                            cranemove.children[0].position,
                                            cranemove.children[1].position,
                                            cranemove.children[2].position], 4, { x: 0 });

                                        TweenMax.to(
                                            container.position, 4,
                                            {
                                                x: -1.1789,
                                                onUpdate: function () { loadProgressUpdate((isNaN(this.progress()) ? 1 : this.progress()), 6, false); },
                                                onComplete: function () {
                                                    TweenMax.to(cranemove.children[1].scale, 2, { y: 2.6 });
                                                    TweenMax.to(cranemove.children[1].position, 2, { y: -1.3 });
                                                    TweenMax.to(cranemove.children[2].position, 2, { y: -0.56 });

                                                    TweenMax.to(container.position, 2,
                                                        {
                                                            y: -0.000099,
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

// Get boat container with crane.
function getBoatContainer(boat) {
    animationInProgress = true;
    loadDeckIsLoadedWith = "boat";

    TweenMax.to(cranemove.position, 2, {
        x: boatX,
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


                        var container = boat.getObjectByName("boatContainer");
                        TweenMax.to(container.position, 2,
                            {
                                y: 0.562,
                                onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                onComplete: function () {

                                    TweenMax.to([
                                        cranemove.children[0].position,
                                        cranemove.children[1].position,
                                        cranemove.children[2].position], 4, { x: 2.1 });

                                    TweenMax.to(
                                        container.position, 4,
                                        {
                                            x: 0.92,
                                            onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                            onComplete: function () {

                                                TweenMax.to([
                                                    container.rotation
                                                ], 2, {
                                                        y: 90 * Math.PI / 180,
                                                        onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                                        onComplete: function () {

                                                            TweenMax.to(cranemove.children[1].scale, 2, { y: 1.9 });
                                                            TweenMax.to(cranemove.children[1].position, 2, { y: -0.765 });
                                                            TweenMax.to(cranemove.children[2].position, 2, { y: -0.365 });
                                                            TweenMax.to(container.position, 2,
                                                                {
                                                                    y: 0.196,
                                                                    onUpdate: function () { loadProgressUpdate(this.progress(), 7); },
                                                                    onComplete: function () {
                                                                        TweenMax.to(container.position, 2,
                                                                            {
                                                                                z: -0.45,
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