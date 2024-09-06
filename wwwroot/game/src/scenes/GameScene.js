// const colors = [0xEABFFF, 0xD580FF];
const colors = [0x4724A1, 0xB387E8, 0x6B1CD5, 0x4D2379];

// #4724A1
// #B387E8
// #6B1CD5
// #4D2379

const TEXT_COLOR = '#3c005a';

class GameScene extends Phaser.Scene {

    constructor() {
        super("GameScene");
    }

    preload() {
        this.datas = this.registry.get('datas')
        this.config = this.registry.get('config')

        // document.getElementById('game-container').style.backgroundColor = this.config.backgroundColor
        document.body.style.backgroundColor = this.config.backgroundColor
        // logo set
        document.getElementById('logo').src = this.config.logoUrl
        // startbuttontext
        this.startButtonText = this.config.startButtonText
        // endButtonText
        this.endButtonText = this.config.endButtonText
        // successText
        this.successText = this.config.successText

        this.spinnable = true

        this.load.image('red', 'public/assets/star3.png');
    }

    create() {
        let {width, height} = this.sys.game.canvas;
        this.ratio = width / 400;

        this.RoundedTextBox(200 * this.ratio, 475 * this.ratio, 50 * this.ratio)
        this.SpinWheel(225 * this.ratio)

    }

    spin() {
        if (!this.spinnable)
            return;

        this.input.off("pointerdown", this.spin, this)

        const particles = this.add.particles(200 * this.ratio, 33.5 * this.ratio, 'red', {
            speed: 100,
            scale: {start: 1, end: 0},
            blendMode: 'ADD'
        });
        particles.depth = 10

        this.spinnable = false
        const angle = 360 / this.datas.length;
        var rounds = Phaser.Math.Between(2, 4);
        var prize = Phaser.Math.Between(0, this.datas.length - 1);
        var spinAngle = 90 + angle / 2 + angle * prize

        this.tweens.add({

            targets: [this.wheel, this.wheeldots],
            angle: -(360 * rounds + spinAngle),
            duration: 3500,
            ease: "Cubic.easeOut",
            callbackScope: this,

            onComplete: function (_) {

                particles.stop()
                const winner = this.datas[prize]
                this.spinnable = true

                this.time.addEvent({
                    delay: 1000,
                    loop: false,
                    callback: () => {
                        this.ShowResult(winner)
                    }
                })

            }
        });
    }

    SpinWheel(h) {

        // BG
        var graphics = this.make.graphics({
            x: 0,
            y: 0,
            add: false
        });
        graphics.fillStyle(0xFFFFFF, 1);
        graphics.fillCircle(182 * this.ratio, 182 * this.ratio, 182 * this.ratio);
        graphics.generateTexture("wheelbg", 182 * 2 * this.ratio, 182 * 2 * this.ratio);
        const bg = this.add.sprite(200 * this.ratio, h, "wheelbg")
        bg.depth = -2

        // UCGEN
        var graphics = this.make.graphics({
            x: 0,
            y: 0,
            add: false
        });
        graphics.fillStyle(0xFFFFFF, 1);
        graphics.fillTriangle(165 * this.ratio, 0, 195 * this.ratio, 0, 180 * this.ratio, 40 * this.ratio)
        graphics.generateTexture("wheelbgTriangle", 180 * 2 * this.ratio, 180 * 2 * this.ratio);
        const bgTriangle = this.add.sprite(200 * this.ratio, h - 20 * this.ratio, "wheelbgTriangle")
        bgTriangle.depth = 1

        // NOKTA
        var graphics = this.make.graphics({
            x: 0,
            y: 0,
            add: false
        });
        graphics.fillStyle(0xFFFFFF, 1);
        graphics.fillCircle(180 * this.ratio, 180 * this.ratio, 25 * this.ratio)
        graphics.generateTexture("wheelbgPoint", 180 * 2 * this.ratio, 180 * 2 * this.ratio);
        const bgPoint = this.add.sprite(200 * this.ratio, h, "wheelbgPoint")

        const parts = []
        const angle = 360 / this.datas.length;

        // looping through each slice
        for (var i = 0; i < this.datas.length; i++) {
            // making a graphic object without adding it to the game
            var graphics = this.make.graphics({
                x: 0,
                y: 0,
                add: false
            });
            // setting graphics fill style
            graphics.fillStyle(colors[i % 4], 1);
            // drawing the slice
            graphics.slice(175 * this.ratio, 175 * this.ratio, 175 * this.ratio, Phaser.Math.DegToRad(i * angle), Phaser.Math.DegToRad((i + 1) * angle), false);
            // filling the slice
            graphics.fillPath();

            graphics.generateTexture("label" + i, 175 * 2 * this.ratio, 175 * 2 * this.ratio);
            const _part = this.add.sprite(0, 0, "label" + i)
            _part.setOrigin(0.5, 0.5)
            _part.depth = -1

            const radianAngle = Phaser.Math.DegToRad(angle * i + angle / 2);

            const text = this.add.text(175 * 0.95 * Math.cos(radianAngle) * this.ratio, 175 * 0.95 * Math.sin(radianAngle) * this.ratio, this.datas[i].title, {
                fontSize: 56,
                color: "#fff",
                fontFamily: "Montserrat",
                resolution: 2
            });
            text.initRTL()
            text.setOrigin(1, 0.5)
            text.angle = angle * i + angle / 2
            parts.push(this.add.container(0, 0, [_part, text]))
        }
        this.wheel = this.add.container(200 * this.ratio, h, parts);
        this.wheel.depth = -2

        // çember üzerindeki noktalar
        var graphics = this.make.graphics({
            x: 0,
            y: 0
        });
        graphics.fillStyle(0xFFFFFF, 1)
        for (var i = 0; i < this.datas.length; i++) {
            graphics.fillCircle((175 + Math.cos(angle * i * Math.PI / 180) * 175) * this.ratio, (175 + Math.sin(angle * i * Math.PI / 180) * 175) * this.ratio, 7 * this.ratio)
        }
        graphics.generateTexture("dot" + i, 175 * 2 * this.ratio, 175 * 2 * this.ratio);
        this.wheeldots = this.add.sprite(200 * this.ratio, h, "dot" + i)

        this.wheel.angle = angle / 3
        this.wheeldots.angle = angle / 3

    }

    RoundedTextBox(x, y, padding) {
        const alttext = this.add.text(x, y, this.startButtonText, {
            fontSize: '72px',
            color: "#ffffff",
            fontFamily: "Montserrat",
            resolution: 2
        });
        alttext.setOrigin(0.5, 0.5);
        const bonsss = alttext.getBounds()
        var graphics = this.make.graphics();
        graphics.fillStyle(0x3c005a, 1);
        graphics.fillRoundedRect(0, 0, bonsss.width + padding, bonsss.height + padding);
        graphics.generateTexture("asdas", bonsss.width + padding, bonsss.height + padding);
        const alttextbg = this.add.sprite(bonsss.centerX, bonsss.centerY, "asdas")
        alttextbg.setOrigin(0.5, 0.5)
        alttextbg.depth = -1
        alttextbg.setInteractive()
        alttextbg.on("pointerdown", () => this.spin())
    }

    ShowResult(winner) {

        this.ResultBg(50 * this.ratio, (50 + 75) * this.ratio, 300 * this.ratio, 250 * this.ratio, 45 * this.ratio)
        this.SuccessText(200 * this.ratio, (50 + 75) * this.ratio, this.successText
            .replace('{title}', winner.title)
            .replace('{code}', winner.code)
            .replace('{description}', winner.description)
            .split('\\n'), winner.code
        )
        this.TextBox(200 * this.ratio, (250 + 75) * this.ratio, 50 * this.ratio, this.endButtonText, () => {
            if (winner.customRedirectUrl != null) {
                window.location.replace(winner.customRedirectUrl)
            } else {
                window.location.replace(winner.redirectUrl)
            }
        });
    }

    ResultBg(x, y, w, h, padding) {
        // var graphics = this.add.graphics({
        //     x: x - (padding / 2),
        //     y: y - (padding / 2)
        // });
        // graphics.fillStyle(0xffffff, 0.7);
        // graphics.fillRoundedRect(0, 0, w + padding, h + padding, 10);
        var graphics = this.add.graphics({
            x: x - ((padding - 5) / 2),
            y: y - ((padding - 5) / 2)
        });
        graphics.fillStyle(0x000000, 0.90);
        graphics.fillRoundedRect(0, 0, w + padding - 5, h + padding - 5, 10 * this.ratio);
    }

    TextBox(textX, textY, padding, endText, onClick) {
        // End Button
        var buttonText = this.add.text(textX, textY, endText, {
            fontSize: '72px',
            fill: '#ffffff',
            align: "center",
            fontFamily: "Montserrat",
            resolution: 2,
            wordWrap: {width: 250 * this.ratio}
        });
        buttonText.setOrigin(0.5, 0.5)
        buttonText.setDepth(1)
        const textBounds = buttonText.getBounds()

        // End Button BG
        var buttonGraphics = this.add.graphics({
            x: textBounds.x - (padding / 2),
            y: textBounds.y - (padding / 2)
        });
        buttonGraphics.fillStyle(0x3c005a, 1);
        buttonGraphics.fillRoundedRect(0, 0, textBounds.width + padding, textBounds.height + padding, 12 * this.ratio);

        buttonGraphics.setInteractive(new Phaser.Geom.Rectangle(0, 0, textBounds.width + padding, textBounds.height + padding), Phaser.Geom.Rectangle.Contains);
        buttonGraphics.on("pointerdown", onClick)
    }

    SuccessText(x, y, successText, code) {
        var width = 0;
        const infoText = this.add.text(x, y, code, {
            fontSize: 88,
            color: "#fff",
            fontFamily: 'Montserrat',
            align: "center",
            wordWrap: {width: 285 * this.ratio},
            resolution: 2
        });
        infoText.depth = 3
        infoText.setOrigin(0.5, 0);
        successText.forEach((text, index) => {
            const infoText = this.add.text(x, y + 50 * this.ratio, text, {
                fontSize: 48,
                color: "#fff",
                fontFamily: 'Montserrat',
                align: "center",
                wordWrap: {width: 285 * this.ratio},
                resolution: 2
            });
            infoText.depth = 3
            infoText.setOrigin(0.5, 0);
            y += infoText.height + 5

        });
    }
}