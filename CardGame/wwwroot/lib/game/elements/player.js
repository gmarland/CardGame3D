class Player {
	constructor(scene) {
		this._scene = scene;

		this._player = new THREE.Group();

		this._width = 10;
		this._height = 2;

		this.draw()
	}

	setPosition(x, y, z) {
		this._location.translateX(x);
		this._location.translateY(y);
		this._location.translateZ(z + (this._height / 2));
	}

	draw() {
		// generate chip
		let locationGeometry = new THREE.CylinderGeometry(this._width, this._width, this._height, 32);
		let locationMaterial = new THREE.MeshLambertMaterial({ color: 0x00ff00 });

		let locationChip = new THREE.Mesh(locationGeometry, locationMaterial);
		locationChip.rotation.x = Math.PI / 2;

		this._player.add(locationChip);

		this._player.translateZ((this._height/2));

		this._scene.add(this._player);
	}
}