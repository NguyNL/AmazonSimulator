var instanceBoat = false;
var firstLoadBoat = true;

class Boat extends THREE.Group {
    constructor(obj = false) {
        super();

        this._obj = obj;
        this._loadState = LoadStates.NOT_LOADING;
        this.init();
    }

    // Initialize the object
    // Load object.
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
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadBoat = false;
            Loading.OBJModel('obj/boat/', 'boat.obj', 'obj/boat/', 'boat.mtl', (mesh) => {
                instanceBoat = mesh;
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