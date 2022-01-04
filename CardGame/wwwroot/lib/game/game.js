function startGame() {
    var viewerContainer = document.getElementById("game-container");

    var _mainRenderer = new Renderer(viewerContainer, "#000000", $(viewerContainer).width(), $(viewerContainer).height());

    // Set up the main scene

    var _mainScene = new Scene(_mainRenderer, window.innerWidth, window.innerHeight);
}