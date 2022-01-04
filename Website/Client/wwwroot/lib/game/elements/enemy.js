class Enemy {
	constructor(scene, location, id, name) {
		this._scene = scene;

		this._location = location;

		this._id = id;
		this._name = name;

		this._selectionLine = null;
		this._enemyGlow = null;

		this._enemy = new THREE.Object3D();
		this._enemy.elementId = id;
		this._enemy.elementType = "enemy";

		this._width = 12;
		this._depth = 15;
		this._height = 1;

		this.draw();
	}

	getId() {
		return this._id;
	}

	getType() {
		return "enemy";
	}

	getCenter() {
		let dimensions = this.getDimensions();

		return {
			x: dimensions.start.x + ((dimensions.end.x - dimensions.start.x) / 2),
			y: dimensions.start.y + ((dimensions.end.y - dimensions.start.y) / 2),
			z: dimensions.start.z + ((dimensions.end.z - dimensions.start.z) / 2),
		};
	}

	getDimensions() {
		var bbox = new THREE.Box3().setFromObject(this._enemy);

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

	getLocationId() {
		return this._location.getId();
	}

	setPosition(x, y, z) {
		this._enemy.position.set(x, y, z + (this._height / 2));
		if (this._enemyGlow) this._enemyGlow.position.set(this._enemy.position.x, this._enemy.position.y, this._enemy.position.z);
	}

	setLocation(location) {
		this._location = location;

		var locationDims = this._location.getDimensions();

		this._enemy.position.set(locationDims.start.x, locationDims.end.y + this._width + 3, (this._height / 2));
		if (this._enemyGlow) this._enemyGlow.position.set(this._enemy.position.x, this._enemy.position.y, this._enemy.position.z);
	}

	drawCurveToPoint(touchPoint) {
		this.removeCurveToPoint();

		this._selectionLine = new THREE.Object3D();

		let center = this.getCenter();

		let middlePointX = center.x + ((touchPoint.x - center.x) / 2),
			middlePointY = center.y + ((touchPoint.y - center.y) / 2);

		let curveQuad = new THREE.QuadraticBezierCurve3(new THREE.Vector3(center.x, center.y, center.z), new THREE.Vector3(middlePointX, middlePointY, center.z + 30), new THREE.Vector3(touchPoint.x, touchPoint.y, center.z));

		let pathPoints = curveQuad.getPoints(100);

		let positions = [];
		for (let i = 0; i < pathPoints.length; i++) positions.push(pathPoints[i].x, pathPoints[i].y, pathPoints[i].z);

		let curvedLineGeometry = new THREE.LineGeometry();
		curvedLineGeometry.setPositions(positions);

		let curvedLineMaterial = new THREE.LineMaterial({
			color: 0xFFFFFF,
			linewidth: 0.003
		});

		this._selectionLine.add(new THREE.Line2(curvedLineGeometry, curvedLineMaterial));

		let sphereGeometry = new THREE.SphereGeometry(0.5, 32, 32);
		let sphereMaterial = new THREE.MeshBasicMaterial({ color: 0xFFFFFF });
		let sphereEndpoint = new THREE.Mesh(sphereGeometry, sphereMaterial)

		sphereEndpoint.translateX(touchPoint.x);
		sphereEndpoint.translateY(touchPoint.y);
		sphereEndpoint.translateZ(center.z);

		this._selectionLine.add(sphereEndpoint);

		this._scene.add(this._selectionLine);
	}

	removeCurveToPoint() {
		if (this._selectionLine) {
			this._scene.remove(this._selectionLine);
			this._selectionLine - null;
		}
	}

	draw() {
		// generate chip
		let locationGeometry = new THREE.BoxGeometry(this._width, this._depth, this._height);
		let locationMaterial = new THREE.MeshLambertMaterial({ color: 0xff0000 });

		let locationChip = new THREE.Mesh(locationGeometry, locationMaterial);

		this._enemy.add(locationChip);

		this._enemy.translateZ((this._height / 2));

		var locationDims = this._location.getDimensions();

		this._enemy.translateX(locationDims.start.x);
		this._enemy.translateY(locationDims.end.y + this._width + 3);

		this._scene.add(this._enemy);
	}

	addSelectedEnemy() {
		let playerGeometry = new THREE.SphereGeometry((this._depth / 2) + 3, 100, 100);
		let playerMaterial = new THREE.MeshPhongMaterial({
			color: 0xffffff,
			opacity: 0.35,
			transparent: true,
			side: THREE.DoubleSide
		});

		this._enemyGlow = new THREE.Mesh(playerGeometry, playerMaterial);

		this._enemyGlow.position.set(this._enemy.position.x, this._enemy.position.y, this._enemy.position.z);

		this._scene.add(this._enemyGlow);
	}

	removeSelectedEnemy() {
		if (this._enemyGlow) {
			this._scene.remove(this._enemyGlow);
			this._enemyGlow - null;
		}
	}
}