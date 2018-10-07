/**
 ** variables.
 **/
var instanceBoat = false;
var firstLoadBoat = true;

/**
 ** Boat model class.
 **/
class Boat extends THREE.Group {
    constructor(obj = false) {
        super();

        this._obj = obj;
        this._loadState = LoadStates.NOT_LOADING;
        this.init();
    }

    // Initialize the object.
    init() {
        if (this._loadState !== LoadStates.NOT_LOADING)
            return;

        this._loadState = LoadStates.LOADING;

        var selfRef = this;

        if (!firstLoadBoat) {
            var checkExistInstanceBoat = setInterval(function () {
                if (instanceBoat) {
                    this.clearInterval(checkExistInstanceBoat);

                    // Clone OBJ model.
                    var clone = instanceBoat.clone();
                    clone.position.set(0, 0, 0);
                    clone.name = "boat";

                    // Boat container.
                    var boatContainer = new THREE.Group();
                    boatContainer.name = "boatContainer";
                    boatContainer.position.y = -0.000099;
                    boatContainer.position.x = -1.1789;
                    boatContainer.position.z = -0.635;

                    // Container.
                    var container = clone.getObjectByName("containerBoat_mesh1355834799.000");
                    container.position.set(1.179, 0, 0.628);
                    container.scale.set(1, 1, 1);

                    // Add mesh.
                    boatContainer.add(container);
                    selfRef.add(boatContainer);
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadBoat = false;
            // Load OBJ model.
            Loading.OBJModel('obj/boat/', 'boat.obj', 'obj/boat/', 'boat.mtl', (mesh) => {
                mesh.name = "boat";

                // Boat container
                var boatContainer = new THREE.Group();
                boatContainer.name = "boatContainer";
                boatContainer.position.y = -0.000099;
                boatContainer.position.x = -1.1789;
                boatContainer.position.z = -0.635;

                // Container
                var container = mesh.getObjectByName("containerBoat_mesh1355834799.000");
                container.position.set(1.179, 0, 0.628);
                container.scale.set(1, 1, 1);

                instanceBoat = mesh;
                // Get OBJ child.
                mesh.traverse(function (child) {
                    if (child instanceof THREE.Mesh) {
                        child.castShadow = true;
                    }
                });
                // Right front head point light.
                addPointLight(selfRef, 0xffffff, -1.0262989 /*x*/, 0.238311 /*y*/, -1.47609 /*z*/, 10 /*intensity*/, 0.5 /*distance*/, 0.3 /*decay*/);

                // lensflares.
                var textureLoader = new THREE.TextureLoader();
                var textureFlare0 = textureLoader.load('/lensflare/lensflare0.png');
                Lensflare(selfRef, textureFlare0, 0xffffff, -1.024389, 0.238311, -1.47079, 100);

                // Add mesh.
                boatContainer.add(container);
                selfRef.add(boatContainer);
                selfRef.add(mesh);
                selfRef._loadState = LoadStates.LOADED;
            });
        }
    }

    // Get loading state
    get loadState() {
        return this._loadState;
    }
    
};