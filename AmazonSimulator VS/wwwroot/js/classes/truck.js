var instanceTruck = false;
var firstLoadTruck = true;

class Truck extends THREE.Group {
    constructor(obj = false) {
        super();

        this._obj = obj;
        this._loadState = LoadStates.NOT_LOADING;
        this.init();
        this.break = false;
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
                    clone.name = "truck";
                    selfRef.add(clone);
                }
            }, 100);
        }
        else {
            firstLoadTruck = false;
            Loading.OBJModel('obj/truck/', 'truck.obj', 'obj/truck/', 'truck.mtl', (mesh) => {
                mesh.name = "truck";
                instanceTruck = mesh;
                selfRef.add(mesh);
                selfRef._loadState = LoadStates.LOADED; 

                // Right front spot light 
                addSpotLight(selfRef, 0xffffff, 0.235, -0.12, 0.19, -0.1, -7, -6, 2, 0.1, 10);

                // Left front spot light 
                addSpotLight(selfRef, 0xffffff, 0.13, -0.12, 0.19, -0.1, -7, -6, 2, 0.1, 10);

                // Right front head point light 
                addPointLight(selfRef, 0xffffff, 0.131699, -0.1697, 0.18085, 20, 0.3, 1);

                // Left front head point light 
                addPointLight(selfRef, 0xffffff, 0.2350989, -0.1697, 0.18085, 20, 0.3, 1);

                // Right back head point light 
                addPointLight(selfRef, 0xff4751, 0.2570988999999998, -0.17479989, 1.06324899, 20, 0.3, 1, "rightBackLight");

                // Left back head point light 
                addPointLight(selfRef, 0xff4751, 0.1200988, -0.17479989, 1.06324899, 20, 0.3, 1, "leftBackLight");
                
            });
        }
    }

    breakLights() {
        if (!this.break) {
            if (this.getObjectByName("rightBackLight"))
                this.getObjectByName("rightBackLight").distance *= 1.5;

            if (this.getObjectByName("leftBackLight"))
                this.getObjectByName("leftBackLight").distance *= 1.5;

            this.break = true;
        }
       
    }

    normalLights() {
        if (this.break) {
            if (this.getObjectByName("rightBackLight"))
                this.getObjectByName("rightBackLight").distance /= 1.5;

            if (this.getObjectByName("leftBackLight"))
                this.getObjectByName("leftBackLight").distance /= 1.5;

            this.break = false;
        }
    }

    // Get loading state
    get loadState() {
        return this._loadState;
    }

};