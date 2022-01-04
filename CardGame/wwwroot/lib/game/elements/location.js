class Location {
	constructor(scene) {
		this._scene = scene;

		this._location = new THREE.Group();

		this._width = 40;
		this._height = 2;

		this.draw()
	}

	setPosition(x, y, z) {
		this._location.translateX(x);
		this._location.translateY(y);
		this._location.translateZ(z + (this._height / 2));
    }

	draw() {
		let locationGeometry = new THREE.BoxGeometry(this._width, this._width, this._height);
		let locationMaterial = new THREE.MeshLambertMaterial({ color: 0xff0000 });
		let locationMesh = new THREE.Mesh(locationGeometry, locationMaterial);

		this._location.add(locationMesh)

		this._location.translateZ((this._height / 2));

		this._scene.add(this._location);
    }
}