class Scene {
	// Create the three.js scene
	constructor(renderer, width, height) {
		this._scene = new THREE.Scene();

		this._renderer = renderer;

		this._mainModel = null;

		this._mainCamera = null;
		this._mainControls = null;

		this._lightingRig = new LightingRig();
		this._scene.add(this._lightingRig.getLights());

		this._width = width;
		this._height = height;

		this._mainCamera = new Camera(this._width, this._height, 0.1, 1000);
		this._mainCamera.setPosition(0, -75, 200);
		this._mainCamera.setLookAt(0, 0, 0);
		this._renderer.startCameraRender(this._scene, this._mainCamera.getCamera());

		this.buildPlayArea();
	}

	getScene() {
		return this._scene;
	}


	getCamera() {
		if (this._mainCamera) return this._mainCamera.getCamera();
		else return null
	}

	getElementById(elementId) {
		for (let i = 0; i < this._players.length; i++) {
			if (this._players[i].getId() === elementId) return this._players[i];
		}

		for (let i = 0; i < this._enemies.length; i++) {
			if (this._enemies[i].getId() === elementId) return this._enemies[i];
		}

		for (let i = 0; i < this._locations.length; i++) {
			if (this._locations[i].getId() === elementId) return this._locations[i];
		}

		return null;
	}

	getTable() {
		return this._table.getObject();
    }

	getElements() {
		let playerElements = [];

		for (let i = 0; i < this._scene.children.length; i++) {
			if (this._scene.children[i].elementType) playerElements.push(this._scene.children[i]);
		}

		return playerElements;
    }

	setSize(width, height) {
		this._width = width;
		this._height = height;

		if (this._mainCamera) this._mainCamera.setAspectRatio(this._width, this._height);

		this._renderer.setSize(width, height);
		this._renderer.setNeedsRendering();
	}

	moveCameraForward(active) {
		this._renderer.moveCameraForward(active);
	}

	moveCameraBackward(active) {
		this._renderer.moveCameraBackward(active);
	}

	moveCameraLeft(distance) {
		let currentPosition = this._mainCamera.getPosition();

		this._mainCamera.setPosition(currentPosition.x - distance, currentPosition.y, currentPosition.z);
	}

	moveCameraRight(distance) {
		let currentPosition = this._mainCamera.getPosition();

		this._mainCamera.setPosition(currentPosition.x + distance, currentPosition.y, currentPosition.z);
	}

	moveCameraUp(distance) {
		let currentPosition = this._mainCamera.getPosition();

		this._mainCamera.setPosition(currentPosition.x, currentPosition.y - distance, currentPosition.z);
	}

	moveCameraDown(distance) {
		let currentPosition = this._mainCamera.getPosition();

		this._mainCamera.setPosition(currentPosition.x, currentPosition.y + distance, currentPosition.z);
	}

	rotateCamera(deltaX, deltaY) {
		this._mainControls.rotate(deltaX, deltaY);

		this._renderer.setNeedsRendering()
	}

	buildPlayArea() {
		this._table = new Table(this._scene);

		this._locations = [];
		this._locationConnections = [];
		this._players = [];
		this._enemies = [];
		this._cards = [];
	}

	// Location methods

	addLocation(id, title, xPos, yPos) {
		this._locations.push(new Location(this._scene, id, title, xPos, yPos));
	}

	addLocationConnection(startLocationId, endLocationId) {
		let sourceLocation = null,
			targetLocation = null;

		for (var i = 0; i < this._locations.length; i++) {
			if (this._locations[i].getId() == startLocationId) sourceLocation = this._locations[i];
			else if (this._locations[i].getId() == endLocationId) targetLocation = this._locations[i];

			if (sourceLocation && targetLocation) break;
		}

		if (sourceLocation && targetLocation) this._locationConnections.push(new LocationConnection(this._scene, sourceLocation, targetLocation));
	}

	getIsLocationsJoined(sourceLocationId, targetLocationId) {
		for (var i = 0; i < this._locationConnections.length; i++) {
			if (this._locationConnections[i].getisLocations(sourceLocationId, targetLocationId)) {
				return true;
            }
		}

		return false;
    }

	getLocationDimensions() {
		let minX = null,
			minY = null,
			minZ = null,
			maxX = null,
			maxY = null,
			maxZ = null;

		for (var i = 0; i < this._locations.length; i++) {
			let dims = this._locations[i].getDimensions();

			if ((minX === null) || (dims.start.x < minX)) minX = dims.start.x;
			if ((minY === null) || (dims.start.y < minY)) minY = dims.start.y;
			if ((minZ === null) || (dims.start.z < minZ)) minZ = dims.start.z;
			if ((maxX === null) || (dims.end.x > maxX)) maxX = dims.end.x;
			if ((maxY === null) || (dims.end.y > maxY)) maxY = dims.end.y;
			if ((maxZ === null) || (dims.end.z > maxZ)) maxZ = dims.end.z;
		}

		return {
			start: {
				x: minX,
				y: minY,
				z: minZ,
			},
			end: {
				x: maxX,
				y: maxY,
				z: maxZ,
			}
		};
    }

	recenterLocations() {
		let locationsDimensions = this.getLocationDimensions();

		let centerX = ((locationsDimensions.end.x - locationsDimensions.start.x) / 2),
			centerY = ((locationsDimensions.end.y - locationsDimensions.start.y) / 2);

		for (var i = 0; i < this._locations.length; i++) {
			let locationObject = this._locations[i].getObject();

			locationObject.translateX((centerX * -1));
			locationObject.translateY(centerY);
		}
	}

	// Player methods

	addPlayer(locationId, id, name) {
		let location = null;

		for (let i = 0; i < this._locations.length; i++) {
			if (this._locations[i].getId() === locationId) {
				location = this._locations[i];
				break;
			}
		}

		if (location) this._players.push(new Player(this._mainCamera, this._scene, location, id, name));
	}

	showPlayerSelected(playerId) {
		console.log("playerId " + playerId);
		for (let i = 0; i < this._players.length; i++) this._players[i].removeSelectedPlayer();
		for (let i = 0; i < this._enemies.length; i++) this._enemies[i].removeSelectedEnemy();

		for (let i = 0; i < this._players.length; i++) {
			if (this._players[i].getId() === playerId) {
				this._players[i].addSelectedPlayer();
				break;
			}
		}
    }

	setPlayerLocation(playerId, locationId) {
		let player = null;

		for (let i = 0; i < this._players.length; i++) {
			if (this._players[i].getId() === playerId) {
				player = this._players[i];
				break;
			}
		}

		if (player) {
			let location = null;

			for (let i = 0; i < this._locations.length; i++) {
				if (this._locations[i].getId() === locationId) {
					location = this._locations[i];
					break;
				}
			}

			if (location) player.setLocation(location);
		}
	}

	applyDamageToEnemy(playerId, enemyId) {

    }

	// Enemy methods

	addEnemy(locationId, id, name) {
		let location = null;

		for (let i = 0; i < this._locations.length; i++) {
			if (this._locations[i].getId() === locationId) {
				location = this._locations[i];
				break;
			}
		}

		if (location) this._enemies.push(new Enemy(this._scene, location, id, name));
	}

	showEnemySelected(enemyId) {
		for (let i = 0; i < this._players.length; i++) this._players[i].removeSelectedPlayer();
		for (let i = 0; i < this._enemies.length; i++) this._enemies[i].removeSelectedEnemy();

		for (let i = 0; i < this._enemies.length; i++) {
			if (this._enemies[i].getId() === enemyId) {
				this._enemies[i].addSelectedEnemy();
				break;
			}
		}
	}

	setEnemyLocation(enemyId, locationId) {
		let enemy = null;

		for (let i = 0; i < this._enemies.length; i++) {
			if (this._enemies[i].getId() === enemyId) {
				enemy = this._enemies[i];
				break;
			}
		}

		if (enemy) {
			let location = null;

			for (let i = 0; i < this._locations.length; i++) {
				if (this._locations[i].getId() === locationId) {
					location = this._locations[i];
					break;
				}
			}

			if (location) enemy.setLocation(location);
		}
	}
}