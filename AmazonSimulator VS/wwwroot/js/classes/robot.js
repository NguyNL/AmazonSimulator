var instanceRobot = false;
var firstLoadRobot = true;

class Robot extends THREE.Group {
    constructor() {
        super();

        //this._obj = obj;
        this._loadState = LoadStates.NOT_LOADING;
        this.init();
    }

    // Initialize the object
    // Load object.
    init() {
        if (this._loadState !== LoadStates.NOT_LOADING)
            return;

        this._loadState = LoadStates.LOADING;

        if (!firstLoadRobot) {
            var selfRef = this;

            var checkExistInstanceRobot = setInterval(function () {
                if (instanceRobot) {
                    this.clearInterval(checkExistInstanceRobot);

                    var clone = instanceRobot.clone();
                    clone.position.set(0, 0, 0);
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadRobot = false;
            var selfRef = this;
            Loading.OBJModel('obj/robot/', 'robot.obj', 'obj/robot/', 'robot.mtl', (mesh) => {
                instanceRobot = mesh;
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