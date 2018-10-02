class Robot extends THREE.Group {
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

        if (this._obj) {
            var clone = this._obj.clone();
            clone.position.x = 0;
            clone.position.z = 0;
            selfRef.add(clone);
        }
        else {
            Loading.OBJModel('obj/robot/', 'robot.obj', 'obj/robot/', 'robot.mtl', (mesh) => {
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