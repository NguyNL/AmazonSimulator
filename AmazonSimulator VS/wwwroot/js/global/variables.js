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
    truckX = 21,
    truckZ = -26,

    // Loading and unloading
    loadDeckIsLoadedWith = false,
    loadProcess = 0,
    unloadProcess = 0,

    animationInProgress = false,
    allProcesses = [],
    allProcessesIndex = 0;