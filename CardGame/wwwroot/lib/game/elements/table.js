class Table {
	constructor(scene) {
		this._scene = scene;

		this._table = new THREE.Group();

		this.draw()
	}

	draw() {
		let tableTopGeometry = new THREE.PlaneGeometry(500, 500);
		let tableTopMaterial = new THREE.MeshLambertMaterial({ color: 0x1a3639, side: THREE.DoubleSide });
		let tableTop = new THREE.Mesh(tableTopGeometry, tableTopMaterial);

		this._table.add(tableTop);

		this._table.translateZ(-1);

		this._scene.add(this._table);
    }
}