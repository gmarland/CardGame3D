class Card {
	constructor(scene) {
		this._scene = scene;

		this._card = new THREE.Group();

		this._width = 15;
		this._height = 20;

		this.draw()
	}

	setPosition(x, y, z) {
		this._location.translateX(x);
		this._location.translateY(y);
		this._location.translateZ(z);
	}

	draw() {
		// Build the front side
		let cardGeometry = new THREE.PlaneGeometry(this._width, this._height);
		let cardMaterial = new THREE.MeshLambertMaterial({ color: 0xffffff, side: THREE.DoubleSide });
		let cardFront = new THREE.Mesh(cardGeometry, cardMaterial);

		this._card.add(cardFront);

		this._scene.add(this._card);
	}
}