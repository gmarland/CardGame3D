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
		this._mainCamera.setPosition(0, -50, 100);
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

	setSize(width, height) {
		this._width = width;
		this._height = height;

		if (this._mainCamera) this._mainCamera.setAspectRatio(this._width, this._height);

		this._renderer.setSize(width, height);
		this._renderer.setNeedsRendering();
	}

	moveCameraForward(active) {
		if (this._viewActive) this._views.moveCameraForward(active);
		else this._renderer.moveCameraForward(active);
	}

	moveCameraBackward(active) {
		if (this._viewActive) this._views.moveCameraBackward(active);
		else this._renderer.moveCameraBackward(active);
	}

	moveCameraLeft(active) {
		if (this._viewActive) this._views.moveCameraLeft(active);
		else this._renderer.moveCameraLeft(active);
	}

	moveCameraRight(active) {
		if (this._viewActive) this._views.moveCameraRight(active);
		else this._renderer.moveCameraRight(active);
	}

	moveCameraUp(active) {
		if (this._viewActive) this._views.moveCameraUp(active);
		else this._renderer.moveCameraUp(active);
	}

	moveCameraDown(active) {
		if (this._viewActive) this._views.moveCameraDown(active);
		else this._renderer.moveCameraDown(active);
	}

	rotateCamera(deltaX, deltaY) {
		this._mainControls.rotate(deltaX, deltaY);

		this._renderer.setNeedsRendering()
	}

	buildPlayArea() {
		this._table = new Table(this._scene);

		this._locations = [];
		this._locations.push(new Location(this._scene));

		this._cards = [];
		this._locations.push(new Card(this._scene));

		this._players = [];
		this._players.push(new Player(this._scene));
    }
}