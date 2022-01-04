class Camera {
	constructor(width, height, near, far) {
		this._camera = new THREE.PerspectiveCamera(75, width / height, near, far);

		this._camera.lookAt(new THREE.Vector3(0, 0, 0));
	}

	getCamera() {
		return this._camera;
	}

	setAspectRatio(width, height) {
		this._camera.aspect = width / height;

		this._camera.updateProjectionMatrix();
	}

	// Set the x/y/z position of the camera
	setPosition(x, y, z) {
		this._camera.position.x = x;
		this._camera.position.y = y;
		this._camera.position.z = z;
	}

	// Set the x/y/z position of the camera
	setRotation(x, y, z) {
		this._camera.rotation.x = x;
		this._camera.rotation.y = y;
		this._camera.rotation.z = z;
	}

	setLookAt(x, y, z) {
		this._camera.lookAt(new THREE.Vector3(x, y, z));
	}
}