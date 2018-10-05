function parseCommand(input = "") {
    return JSON.parse(input);
}

function addPointLight(object, color, x, y, z, intensity, distance, decay, name = "") {
    var pointLight = new THREE.PointLight(color, intensity, distance);

    pointLight.position.set(x, y, z);
    pointLight.castShadow = true;

    pointLight.decay = decay;
    pointLight.name = name;

    object.add(pointLight);
}

function addSpotLight(object, color, x, y, z, tx, ty, tz, decay, intensity, distance) {
    var spotLight = new THREE.SpotLight(color, intensity, distance);

    spotLight.position.set(x, y, z);
    spotLight.target.position.set(tx, ty, tz);

    spotLight.decay = decay;
    spotLight.castShadow = true;

    object.add(spotLight);
    object.add(spotLight.target);
}