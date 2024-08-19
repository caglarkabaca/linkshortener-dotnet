class PreloadScene extends Phaser.Scene {
    constructor() {
        super("PreloadScene")
    }

    preload() {

    }

    create() {

        const searchParams = new URLSearchParams(window.location.search);
        var uniqueCode = searchParams.get('code')
        if (uniqueCode.length <= 0)
            return
        var url = "https://localhost:7031/CampaignCoupon/" + uniqueCode
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
                        endButtonText: "Kuponu kullanmak için tıkla",
                        successText: "Kazandınız\\n{title}\\nKodunuz: {code}\\n{description}",
                    }
                }
                this.registry.set('config', json.setting)

                document.getElementById("loader").remove()
                document.getElementById("mainApp").style.display = "block"

                this.scene.start("GameScene");
            })
    }
}