class PreloadScene extends Phaser.Scene {
    constructor() {
        super("PreloadScene")
    }

    preload() {
        this.load.script('webfont', 'https://ajax.googleapis.com/ajax/libs/webfont/1.6.26/webfont.js');
    }

    create() {

        WebFont.load({
            google: {
                families: [ 'Montserrat']
            },
        });
        
        const searchParams = new URLSearchParams(window.location.search);
        var uniqueCode = searchParams.get('code')
        if (uniqueCode.length <= 0)
            return
        const host = window.location.origin;
        var url = host + "/CampaignCoupon/" + uniqueCode
        this.spinnable = false
        fetch(url)
            .then((response) => response.json())
            .then((json) => {
                var datas = []
                json.coupons.forEach(data => {
                    datas.push(data)
                });
                this.registry.set('datas', datas)

                if (json.setting == null) {
                    json.setting = {
                        logoUrl: "https://winfluencer.app/wp-content/uploads/w-logo.png",
                        backgroundColor: "#080950",
                        startButtonText: "Çevirmek için tıkla",
                        endButtonText: "Siteye git",
                        successText: "Tebrikler {title} adlı kuponu kazandınız!\n{code} kodunu ile kuponunuzu kullanabilirsiniz.",
                    }
                }
                this.registry.set('config', json.setting)

                document.getElementById("loader").remove()
                document.getElementById("mainApp").style.display = "flex"

                this.scene.start("GameScene");
            })
    }
}