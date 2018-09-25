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
        
        if (this.obj) selfRef.add(this._obj.clone());
        else {
            var selfRef = this;
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