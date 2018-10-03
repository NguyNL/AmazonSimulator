var instanceTruck = false;
var firstLoadTruck = true;

class Truck extends THREE.Group {
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

        if (!firstLoadTruck) {
            var checkExistInstanceTruck = setInterval(function () {
                if (instanceTruck) {
                    this.clearInterval(checkExistInstanceTruck);

                    var clone = instanceTruck.clone();
                    clone.position.set(0, 0, 0);
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadTruck = false;
            Loading.OBJModel('obj/truck/', 'truck.obj', 'obj/truck/', 'truck.mtl', (mesh) => {
                instanceTruck = mesh;
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