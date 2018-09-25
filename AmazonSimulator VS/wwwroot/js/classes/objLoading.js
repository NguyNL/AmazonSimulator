/**
 ** LOAD MODEL CLASS
 **/
class Loading {
    // Load OBJ model
    static OBJModel(modelPath, modelName, texturePath, textureName, callback) {
        // Make .MTL loader instance for textures
        var mtlLoader = new THREE.MTLLoader();

        // Set texture paths
        mtlLoader.setTexturePath(texturePath);
        mtlLoader.setPath(texturePath);

        // Load the texture file
        mtlLoader.load(textureName, function (materials) {
            materials.preload();

            // Make a .obj loader instance
            var objLoader = new THREE.OBJLoader();

            // Set object path
            objLoader.setMaterials(materials);
            objLoader.setPath(modelPath);

            // Load the object
            objLoader.load(modelName, function (object) {
                // Run the callback function with loaded object parameter
                callback(object);
            });
        });
    }
}