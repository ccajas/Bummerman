# Bummerman

A Bomberman clone!

This game requires MonoGame 3.2 to build. The build target for this project is Windows/Windows 8.

Game logic is driven by *Meteor ECS*, a custom Entity Component System framework. This game was built as a test to use the framework in a practical situation.

Sprite assets are currently not included (using copyrighted sprites as placeholders). You can wait for original sprites to be added soon, or use your own spritesheet.

## Currently

* Keyboard and joypad button support
  * Uses keyboard as default
* Only one power up (extra bomb upgrade)
  * Custom power-ups can easily be implemented
* Up to 4 players

## In progress

* Improved control mapping system
  * Add joystick support
* Add AI players
* Basic client-server networking
* Custom sprite assets
