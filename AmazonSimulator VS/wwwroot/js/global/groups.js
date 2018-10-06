/**
 ** GROUPS
 **/
var platformGroup = new THREE.Group();

//Truck
var truckGroup = new THREE.Group();
truckGroup.scale.set(20, 20, 20);
truckGroup.position.y = 0;
truckGroup.position.z = -26;

//Boat
var boatGroup = new THREE.Group();
boatGroup.scale.set(20, 20, 20);
boatGroup.position.y = 0;
boatGroup.position.x = 2.15;
boatGroup.position.z = -2.9;

//var boatContainer = new THREE.Group();

// CraneMove
var cranemove = new THREE.Group();
cranemove.scale.set(20, 20, 20);
cranemove.position.x = 35;

// Warehouse
var warehouse = new THREE.Group();
warehouse.position.x = 17;
warehouse.position.y = -4.39;
warehouse.position.z = -30;

// warehouseGrid
var warehouseGrid = new THREE.Group();
warehouseGrid.rotation.y = 90 * Math.PI / 180;
warehouseGrid.position.x = 3.8;
warehouseGrid.position.z = 32;
warehouse.add(warehouseGrid);

// Robot
var robotsGroup = new THREE.Group();
robotsGroup.position.y = 0.2;
robotsGroup.scale.set(0.06, 0.06, 0.06);
warehouseGrid.add(robotsGroup);

// Rack
var racksGroup = new THREE.Group();
racksGroup.position.y = 1.57;
racksGroup.scale.set(0.06, 0.06, 0.06);
warehouseGrid.add(racksGroup);

// Doors
var doorLeft = new THREE.Group();
var doorRight = new THREE.Group();

doorRight.position.z = -1.48;
doorLeft.position.z = -1.48;

doorRight.position.y = 7;
doorLeft.position.y = 7;

doorRight.position.x = 9.856;
doorLeft.position.x = 9.856;

doorRight.scale.set(12, 12, 12);
doorLeft.scale.set(12, 12, 12);

warehouseGrid.add(doorLeft);
warehouseGrid.add(doorRight);