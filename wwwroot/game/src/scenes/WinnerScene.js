const TEXT2_COLOR = '#EABFFF';

class WinnerScene extends Phaser.Scene {

    constructor() {
        super("WinnerScene");
    }

    preload() {
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
    }

    create() {
        const winner = this.registry.get('winner')

        this.SuccessText(200, 50, this.successText
            .replace('{title}', winner.title)
            .replace('{code}', winner.code)
            .replace('{description}', winner.description)
            .split('\\n')
        )
        this.TextBox(200, 375, 50, this.endButtonText);

        this.input.on("pointerdown", function () {
            if (winner.customRedirectUrl != null) {
                window.location.replace(winner.customRedirectUrl)
            }
            else {
                window.location.replace(winner.redirectUrl)
            }
        }, this)
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
                fontFamily: 'Arial',
                align: "center",
                wordWrap: { width: 300 }
            });
            infoText.setOrigin(0.5, 0);
            y += infoText.height + 5
        });
    }
}