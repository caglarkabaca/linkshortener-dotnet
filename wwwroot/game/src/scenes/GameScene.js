const colors = [0xEABFFF, 0xD580FF];
const TEXT_COLOR = '#3c005a';


class GameScene extends Phaser.Scene {

    constructor() {
        super("GameScene");
    }

    preload() {
        this.datas = this.registry.get('datas')
        this.config = this.registry.get('config')

        document.getElementById('game-container').style.backgroundColor = this.config.backgroundColor
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
        this.RoundedTextBox(200, 450, 50)
        this.SpinWheel(200)

        this.input.on("pointerdown", this.spin, this)
    }

    spin() {
        if (!this.spinnable)
            return;

        this.input.off("pointerdown", this.spin, this)

        const particles = this.add.particles(200, 20, 'red', {
            speed: 100,
            scale: { start: 1, end: 0 },
            blendMode: 'ADD'
        });
        particles.depth = 10

        this.spinnable = false
        const angle = 360 / this.datas.length;
        var rounds = Phaser.Math.Between(2, 4);
        var prize = Phaser.Math.Between(0, this.datas.length - 1);

        this.tweens.add({

            targets: [this.wheel],
            angle: 360 * rounds + (240 - prize * angle) % 360,
            duration: 2000,
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

        var graphics = this.make.graphics({
            x: 0,
            y: 0,
            add: false
        });
        graphics.fillStyle(0xFFBFFF, 1);
        graphics.fillCircle(180, 180, 180);
        graphics.generateTexture("wheelbg", 180 * 2, 180 * 2);
        const bg = this.add.sprite(200, h, "wheelbg")
        var graphics = this.make.graphics({
            x: 0,
            y: 0,
            add: false
        });
        graphics.fillStyle(TEXT_COLOR, 1);
        graphics.fillTriangle(170, 0, 190, 0, 180, 15)
        graphics.generateTexture("wheelbgTriangle", 180 * 2, 180 * 2);
        const bgTriangle = this.add.sprite(200, h, "wheelbgTriangle")
        bgTriangle.depth = 1

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
            graphics.fillStyle(colors[i % 2], 1);
            // drawing the slice
            graphics.slice(175, 175, 175, Phaser.Math.DegToRad(i * angle), Phaser.Math.DegToRad((i + 1) * angle), false);
            // filling the slice
            graphics.fillPath();
            graphics.generateTexture("label" + i, 175 * 2, 175 * 2);
            const _part = this.add.sprite(0, 0, "label" + i)
            _part.setOrigin(0.5, 0.5)
            _part.depth = -1

            const radianAngle = Phaser.Math.DegToRad(angle * i + angle / 2);

            const text = this.add.text(175 * 0.5 * Math.cos(radianAngle), 175 * 0.5 * Math.sin(radianAngle), this.datas[i].title, {
                fontSize: '22px',
                color: TEXT_COLOR,
                fontFamily: 'Arial',
                strokeThickness: 5
            });
            text.setOrigin(0.5, 0.5)
            text.angle = angle * i + angle / 2
            parts.push(this.add.container(0, 0, [_part, text]))
        }
        this.wheel = this.add.container(200, h, parts);
    }

    RoundedTextBox(x, y, padding) {
        const alttext = this.add.text(x, y, this.startButtonText, {
            fontSize: '28px',
            color: "#ffffff",
            fontFamily: 'Arial'
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
    }

    ShowResult(winner) {

        this.ResultBg(50, 50 + 75, 300, 375, 40)
        this.SuccessText(200, 50 + 75, this.successText
            .replace('{title}', winner.title)
            .replace('{code}', winner.code)
            .replace('{description}', winner.description)
            .split('\\n')
        )
        this.TextBox(200, 375 + 75, 50, this.endButtonText);

        this.input.on("pointerdown", function () {
            if (winner.customRedirectUrl != null) {
                window.location.replace(winner.customRedirectUrl)
            }
            else {
                window.location.replace(winner.redirectUrl)
            }
        }, this)
    }

    ResultBg(x, y, w, h, padding) {
        var graphics = this.add.graphics({
            x: x - (padding / 2),
            y: y - (padding / 2)
        });
        graphics.fillStyle(0xFFBFFF, 1);
        graphics.fillRoundedRect(0, 0, w + padding, h + padding, 12);
    }

    TextBox(textX, textY, padding, endText) {
        // End Button
        var buttonText = this.add.text(textX, textY, endText, {
            fontSize: '24px',
            fill: '#ffffff',
            align: "center",
            wordWrap: { width: 250 }
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
        buttonGraphics.fillRoundedRect(0, 0, textBounds.width + padding, textBounds.height + padding, 12);
    }

    SuccessText(x, y, successText) {
        successText.forEach((text, index) => {
            var fontSize = 64 - 16 * index;
            if (fontSize < 24)
                fontSize = 24
            const infoText = this.add.text(x, y, text, {
                fontSize: fontSize,
                color: TEXT_COLOR,
                strokeThickness: 5,
                fontFamily: 'Arial',
                align: "center",
                wordWrap: { width: 300 }
            });
            infoText.setOrigin(0.5, 0);
            y += infoText.height + 5
        });
    }
}