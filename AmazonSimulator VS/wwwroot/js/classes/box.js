var instanceBox = false;
var firstLoadBox = true;

class Box extends THREE.Group {

    constructor() {
        super();
        
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

        if (!firstLoadBox) {
            var checkExistInstance = setInterval(function () {
                if (instanceBox) {
                    this.clearInterval(checkExistInstance);

                    var clone = instanceBox.clone();
                    clone.position.set(0, 0, 0);
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadBox = false;
            Loading.OBJModel('obj/cardboard_box/', 'cardboard_box.obj', 'obj/cardboard_box/', 'cardboard_box.mtl', (mesh) => {
                instanceBox = mesh;
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