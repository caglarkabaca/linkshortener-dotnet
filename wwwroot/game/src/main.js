import {Game} from 'phaser';

import {GameScene} from './scenes/GameScene.js'
import {WinnerScene} from './scenes/WinnerScene.js'
import {PreloadScene} from './scenes/PreloadScene.js';

//  Find out more information about the Game Config at: https://newdocs.phaser.io/docs/3.70.0/Phaser.Types.Core.GameConfig
const config = {
    type: Phaser.WebGl,
    width: 400,
    height: 600,
    parent: 'game-container',
    transparent: true,
    scene: [
        PreloadScene,
        GameScene,
        WinnerScene
    ]
};

export default new Game(config);

