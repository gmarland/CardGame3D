class Location {
	constructor(scene, id, title, xPos, yPos) {
		this._scene = scene;

		this._id = id;
		this._title = title;
		this._xPos = xPos;
		this._yPos = yPos;

		this._location = new THREE.Object3D();
		this._location.elementId = id;
		this._location.elementType = "location";

		this._width = 30;
		this._height = 2;
		this._spacing = 90;

		this.draw()
	}

	draw() {
		let locationGeometry = new THREE.BoxGeometry(this._width, this._width, this._height);
		let locationMaterial = new THREE.MeshLambertMaterial({ color: 0x7a96ea });
		let locationMesh = new THREE.Mesh(locationGeometry, locationMaterial);

		this._location.add(locationMesh)

		this._location.translateZ((this._height / 2));

		this._location.translateX((this._xPos - 1) * this._width + ((this._xPos - 1) * this._spacing));
		this._location.translateY(((this._yPos - 1) * this._width + ((this._yPos - 1) * this._spacing)) * -1);

		this._scene.add(this._location);
	}

	getId() {
		return this._id;
	}

	getType() {
		return "location";
	}

	getObject() {
		return this._location;
	}

	getCenter() {
		let dimensions = this.getDimensions();

		return {
			x: dimensions.start.x + ((dimensions.end.x - dimensions.start.x)/2),
			y: dimensions.start.y + ((dimensions.end.y - dimensions.start.y) / 2),
			z: dimensions.start.z + ((dimensions.end.z - dimensions.start.z) / 2),
		};
    }

	getDimensions() {
		var bbox = new THREE.Box3().setFromObject(this._location);

		return {
			start: {
				x: bbox.min.x,
				y: bbox.min.y,
				z: bbox.min.z
			},
			end: {
				x: bbox.max.x,
				y: bbox.max.y,
				z: bbox.max.z
			}
		};
	}

	setPosition(x, y, z) {
		this._location.translateX(x);
		this._location.translateY(y);
		this._location.translateZ(z + (this._height / 2));
	}
}