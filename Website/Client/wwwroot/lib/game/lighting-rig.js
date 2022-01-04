class LightingRig {
	constructor() {
		this._lights = new THREE.Object3D();
		this._lights.name = "LightingRig";

		var centerX = 0,
			centerY = 0,
			centerZ = 0;

		var radius = 300;

		this._createlight(new THREE.Color("#FFFFFF"), 0.5, centerX, (centerY + (radius * 2)), centerZ, centerX, centerY, centerZ);
		this._createlight(new THREE.Color("#FFFFFF"), 0.5, centerX, (centerY - (radius * 2)), centerZ, centerX, centerY, centerZ);
		this._createlight(new THREE.Color("#FFFFFF"), 0.5, (centerX + (radius * 2)), centerY, centerZ, centerX, centerY, centerZ);
		this._createlight(new THREE.Color("#FFFFFF"), 0.5, (centerX - (radius * 2)), centerY, centerZ, centerX, centerY, centerZ);
		this._createlight(new THREE.Color("#FFFFFF"), 0.5, centerX, centerY, (centerZ + (radius * 2)), centerX, centerY, centerZ);
		this._createlight(new THREE.Color("#FFFFFF"), 0.5, centerX, centerY, (centerZ - (radius * 2)), centerX, centerY, centerZ);

		this._lights.add(new THREE.AmbientLight(0x404040));
	}

	_createlight(color, intensity, positionX, positionY, positionZ, lookAtX, lookAtY, lookAtZ) {
		var directionalLight = new THREE.DirectionalLight(new THREE.Color(color), intensity);

		directionalLight.position.set(positionX, positionY, positionZ);
		directionalLight.lookAt(new THREE.Vector3(lookAtX, lookAtY, lookAtZ));

		this._lights.add(directionalLight);
	}

	getLights() {
		return this._lights;
	}

	setIntensity(intensity) {
		for (var i = 0; i < _lights.children.length; i++) {
			this._lights.children[i].intensity = intensity;
		}
	}
};