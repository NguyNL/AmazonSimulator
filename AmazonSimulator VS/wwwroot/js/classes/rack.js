/**
 ** variables.
 **/
var instanceRack = false;
var firstLoadRack = true;


/**
 ** Rack model class.
 **/
class Rack extends THREE.Group {
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

        if (!firstLoadRack) {
            var checkExistInstanceRack = setInterval(function () {
                if (instanceRack) {
                    this.clearInterval(checkExistInstanceRack);

                    var clone = instanceRack.clone();
                    clone.position.set(0, 0, 0);
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadRobot = false;
            Loading.OBJModel('obj/storage_rack/', 'rackpoly.obj', 'obj/storage_rack/', 'rackpoly.mtl', (mesh) => {
                instanceRack = mesh;
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