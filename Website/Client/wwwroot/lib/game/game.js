let _viewerContainer,
    _mainRenderer,
    _mainScene;

let _raycaster = new THREE.Raycaster(),
    _mouse = new THREE.Vector2();

let _selectedElement = null;

let _mouseDown = false;

function startGame() {
    _viewerContainer = document.getElementById("game-container");

    _mainRenderer = new Renderer(_viewerContainer, "#000000", $(_viewerContainer).width(), $(_viewerContainer).height());

    // Set up the main scene

    _mainScene = new Scene(_mainRenderer, window.innerWidth, window.innerHeight);

    this.bindActions();
}

function addLocation(id, title, xPos, yPos) {
    _mainScene.addLocation(id, title, xPos, yPos)
}

function addLocationConnection(sourceLocationId, targetLocationId) {
    _mainScene.addLocationConnection(sourceLocationId, targetLocationId);
}

function addPlayer(locationId, id, name) {
    _mainScene.addPlayer(locationId, id, name);
}

function showPlayerSelected(enemyId) {
    _mainScene.showPlayerSelected(enemyId);
}

function setPlayerLocation(playerId, locationId) {
    _mainScene.setPlayerLocation(playerId, locationId);
}

function addEnemy(locationId, id, name) {
    _mainScene.addEnemy(locationId, id, name);
}

function showEnemySelected(enemyId) {
    _mainScene.showEnemySelected(enemyId);
}

function setEnemyLocation(enemyId, locationId) {
    _mainScene.setEnemyLocation(enemyId, locationId);
}

function showEnemySelected(enemyId) {
    _mainScene.showEnemySelected(enemyId);
}

function recenterLocations() {
    _mainScene.recenterLocations();
}

function bindActions() {
    let that = this;

    let lastX = null,
        lastY = null;

    window.addEventListener('resize', function () {
        _mainScene.setSize(window.innerWidth, window.innerHeight);
    }, false);

    $(_viewerContainer).mousedown(function (e) {
        e.stopPropagation();
        e.preventDefault();

        _selectedElement = null;

        _mouse.set((e.clientX / window.innerWidth) * 2 - 1, -(e.clientY / window.innerHeight) * 2 + 1);
        _raycaster.setFromCamera(_mouse, _mainScene.getCamera());

        let sceneChildren = _mainScene.getElements();

        let intersects = _raycaster.intersectObjects(sceneChildren, true);

        let intersected = null;

        for (let i = 0; i < intersects.length; i++) {
            if ((intersected === null) || (intersects[i].distance < intersected.distance)) intersected = intersects[i];
        }

        if (intersected) _selectedElement = _mainScene.getElementById(intersected.object.parent.elementId);
        
        that._mouseDown = true;

        lastX = e.pageX;
        lastY = e.pageY;
    });

    $(_viewerContainer).mousemove(function (e) {
        e.stopPropagation();
        e.preventDefault();

        if (that._mouseDown) {
            if (_selectedElement === null) {
                let moveX = (e.pageX - lastX);
                let moveY = (e.pageY - lastY);

                if (moveX !== 0) {
                    if (moveX > 0) _mainScene.moveCameraLeft(Math.abs(moveX) / 2);
                    else _mainScene.moveCameraRight(Math.abs(moveX) / 2);
                }

                if (moveY !== 0) {
                    if (moveY > 0) _mainScene.moveCameraDown(Math.abs(moveY) / 2);
                    else _mainScene.moveCameraUp(Math.abs(moveY) / 2);
                }
            }
            else if (_selectedElement.getType() === "player") {
                _mouse.set((e.clientX / window.innerWidth) * 2 - 1, -(e.clientY / window.innerHeight) * 2 + 1);
                _raycaster.setFromCamera(_mouse, _mainScene.getCamera());

                let intersects = _raycaster.intersectObject(_mainScene.getTable(), true);

                if (intersects.length > 0) _selectedElement.drawCurveToPoint(new THREE.Vector2(intersects[0].point.x, intersects[0].point.y));
            }
        }

        lastX = e.pageX;
        lastY = e.pageY;
    });

    $(_viewerContainer).mouseup(function (e) {
        e.stopPropagation();
        e.preventDefault();

        that._mouseDown = false;

        lastX = null;
        lastY = null;

        if ((_selectedElement) && (_selectedElement.getType() === "player")) {
            let sceneChildren = _mainScene.getElements();

            let intersects = _raycaster.intersectObjects(sceneChildren, true);

            let intersected = null;

            for (let i = 0; i < intersects.length; i++) {
                if ((intersected === null) || (intersects[i].distance < intersected.distance)) intersected = intersects[i];
            }

            if (intersected) {
                let selectedElement = _mainScene.getElementById(intersected.object.parent.elementId);

                if (selectedElement) {
                    if (selectedElement.getId() !== _selectedElement.getId()) {
                        if (selectedElement.getType() === "location") {
                            if (_selectedElement.getLocationId() === selectedElement.getId()) {
                                searchPlayerLocation(_selectedElement.getId(), selectedElement.getId())
                            }
                            else if (_mainScene.getIsLocationsJoined(_selectedElement.getLocationId(), selectedElement.getId())) {
                                movePlayerLocation(_selectedElement.getId(), selectedElement.getId());
                            }
                        }
                        else if (selectedElement.getType() === "enemy") {
                            applyDamageToEnemy(_selectedElement.getId(), selectedElement.getId());
                        }
                    }
                }
            }

            _selectedElement.removeCurveToPoint();
        }

        _selectedElement = null;
    });
}