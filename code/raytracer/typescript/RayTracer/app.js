function exec() {
    var canv = document.createElement("canvas");
    canv.width = 256;
    canv.height = 256;
    document.body.appendChild(canv);
    var ctx = canv.getContext("2d");
    for (var y = 0; y < canv.height; y++) {
        for (var x = 0; x < canv.width; x++) {
            ctx.fillStyle = "rgb(255,0,0)";
            ctx.fillRect(x, y, x + 1, y + 1);
        }
    }
}
exec();
//# sourceMappingURL=app.js.map