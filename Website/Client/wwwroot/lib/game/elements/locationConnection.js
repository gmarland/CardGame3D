class LocationConnection {
    constructor(scene, sourceLocation, targetLocation) {
        this._scene = scene;

        this._sourceLocation = sourceLocation;
        this._targetLocation = targetLocation;

        this._locationConnectionObject = new THREE.Object3D();

        this.draw();
    }

    getisLocations(sourceLocationId, targetLocationId) {
        return (((this._sourceLocation.getId() === sourceLocationId) && (this._targetLocation.getId() == targetLocationId)) ||
                ((this._sourceLocation.getId() === targetLocationId) && (this._targetLocation.getId() == sourceLocationId)));
    }

    draw() {

        let positions = [];

        let sourceLocationDims = this._sourceLocation.getDimensions();
        let sourceLocationCenter = this._sourceLocation.getCenter();

        let targetLocationDims = this._targetLocation.getDimensions();
        let targetLocationCenter = this._targetLocation.getCenter();

        positions.push(sourceLocationCenter.x, sourceLocationCenter.y, sourceLocationDims.start.z);
        positions.push(targetLocationCenter.x, targetLocationCenter.y, targetLocationDims.start.z);

        let lineGeometry = new THREE.LineGeometry();
        lineGeometry.setPositions(positions);

        let lineMaterial = new THREE.LineMaterial({
            color: 0xFFFFFF,
            linewidth: 0.003
        });

        this._locationConnectionObject.add(new THREE.Line2(lineGeometry, lineMaterial));

        this._scene.add(this._locationConnectionObject);
    }
}