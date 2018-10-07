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

                    var clone = instanceBoat.clone();
                    clone.position.set(0, 0, 0);
                    clone.name = "boat";
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadBoat = false;
            Loading.OBJModel('obj/boat/', 'boat.obj', 'obj/boat/', 'boat.mtl', (mesh) => {
                mesh.name = "boat";
                instanceBoat = mesh;
                mesh.traverse(function (child) {
                    if (child instanceof THREE.Mesh) {
                        child.castShadow = true;
                    }
                });
                // Right front head point light 
                addPointLight(selfRef, 0xffffff, -1.0262989 /*x*/, 0.238311 /*y*/, -1.47609 /*z*/, 10 /*intensity*/, 0.5 /*distance*/, 0.3 /*decay*/);

                // lensflares
                var textureLoader = new THREE.TextureLoader();
                var textureFlare0 = textureLoader.load('/lensflare/lensflare0.png');
                Lensflare(selfRef, textureFlare0, 0xffffff, -1.024389, 0.238311, -1.47079, 100);
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