class Renderer {
	constructor(viewerContainer, backgroundColor, width, height) {
		this._renderer = new THREE.WebGLRenderer({ antialias: true });

		this._viewerContainer = viewerContainer;

		this._scene = null;
		this._controls = null;
		this._camera = null;

		this._moveCameraForward = false;
		this._moveCameraBackward = false;
		this._moveCameraLeft = false;
		this._moveCameraRight = false;
		this._moveCameraUp = false;
		this._moveCameraDown = false;

		this._preRender = null;
		this._preRenderArgs = null;
		this._postRender = null;
		this._postRenderArgs = null;

		this._renderingStarted = false;
		this._needsRendering = true;

		this.setBackground(backgroundColor);

		this._width = width;
		this._height = height;

		this.setSize(width, height);

		this._viewerContainer.appendChild(this._renderer.domElement);
	}

	getParentContainer() {
		return this._viewerContainer;
	}

	getDOMElement() {
		return this._renderer.domElement;
	}

	getSize() {
		return {
			width: this._width,
			height: this._height
		};
	}

	setBackground(color) {
		// Set the background color of the renderer. Create a custom color with a Hex value.
		this._renderer.setClearColor(new THREE.Color(color));
	}

	setSize(width, height) {
		this._width = width;
		this._height = height;

		this._renderer.setSize(width, height);
	}

	setNeedsRendering() {
		this._needsRendering = true;
	}

	moveCameraForward(active) {
		this._moveCameraForward = active;
	}

	moveCameraBackward(active) {
		this._moveCameraBackward = active;
	}

	moveCameraLeft(active) {
		this._moveCameraLeft = active;
	}

	moveCameraRight(active) {
		this._moveCameraRight = active;
	}

	moveCameraUp(active) {
		this._moveCameraUp = active;
	}

	moveCameraDown(active) {
		this._moveCameraDown = active;
	}

	_render() {
		requestAnimationFrame(() => this._render());

		if (this._scene && this._camera) {
			if (this._preRender) {
				if (this._preRenderArgs) this._preRender(this._preRenderArgs);
				else this._preRender();
			}

			this._renderer.render(this._scene, this._camera);

			if (this._postRender) {
				if (this._postRenderArgs) this._postRender(this._postRenderArgs);
				else this._postRender();
			}
		}
	}

	startCameraRender(scene, camera, preRender, preRenderArgs, postRender, postRenderArgs) {
		this._setRendering(scene, camera, preRender, preRenderArgs, postRender, postRenderArgs);
	}

	_setRendering(scene, camera, preRender, preRenderArgs, postRender, postRenderArgs) {
		this._scene = scene;
		this._camera = camera;

		this._preRender = preRender;
		this._preRenderArgs = preRenderArgs;
		this._postRender = postRender;
		this._postRenderArgs = postRenderArgs;

		this._render();
	}
};