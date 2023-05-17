# GameLoader Script

This script is part of a Unity game project and is responsible for saving and loading game data, including player position, enemy positions, pickup states, and weapon information.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Usage](#usage)
- [Dependencies](#dependencies)
- [Contributing](#contributing)
- [License](#license)

## Overview

The GameLoader script is a crucial component of the game project. It handles the saving and loading of game data to provide players with a seamless experience when exiting and reentering the game. By saving and loading player progress, enemy positions, pickup states, and weapon information, the script ensures that players can continue their game from where they left off.

## Features

The GameLoader script offers the following features:

- Saving and loading player position, rotation, health, and weapon information.
- Saving and loading enemy positions and rotations.
- Saving and loading pickup states.
- Saving and loading swappable weapon states.
- Saving and loading physics objects.
- Integrating with the game's skydome and time system.
- Randomized game start location.
- Boss fight mode and special level settings.
- Gender selection for the player character.
- Mod support for weapons.

## Usage

To use the GameLoader script in your Unity project, follow these steps:

1. Attach the GameLoader script to an appropriate GameObject in your scene.
2. Customize the script's variables and settings according to your game's requirements.
3. Ensure that all necessary components and references are correctly assigned in the Inspector.
4. Build and run your game.
5. The script will handle the saving and loading of game data automatically based on your settings.

## Dependencies

The GameLoader script has the following dependencies:

- Unity Engine
- UnityEngine.AI
- EnemyAI script

Please make sure to include these dependencies in your Unity project to use the GameLoader script effectively.

## Contributing

Contributions to the GameLoader script are welcome. If you find any issues or have suggestions for improvements, please open an issue or submit a pull request on the project's GitHub repository. Your contributions will help enhance the script and benefit the community.

## License

The GameLoader script is provided under the [MIT License](LICENSE). You are free to use, modify, and distribute the script in your own projects.
