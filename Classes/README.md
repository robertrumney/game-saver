# Unity Game Data Save and Load System

This project provides a game data save and load system for Unity games. It includes a collection of serialized classes that can be used to save and load various aspects of game data such as game state, weather, character information, player inventory, game progress, and more.

## Features

- Saving and loading game data with customizable fields.
- Support for saving and loading level progress, including player position and object states.
- Ability to save and load player inventory with item details and quantities.
- Support for game progress tracking and conditions checking.
- Serialization of Vector3 and Quaternion data types for saving and loading positions and rotations.

## Getting Started

To use the game data save and load system in your Unity project, follow these steps:

1. Import the provided classes into your Unity project's scripts folder.
2. Create an instance of the `SaveData` class to represent your game data.
3. Customize the fields in the `SaveData` class to match your game's requirements.
4. Use the `JsonUtility` class provided by Unity to serialize and deserialize the `SaveData` object to and from JSON format.
5. Implement the necessary logic to save and load the game data in your game.

## Class Descriptions

The following are the main classes included in the project:

### SaveData

This class represents the overall game data that needs to be saved. It contains various fields such as game state, weather, character information, player inventory, game progress, and more. It also includes methods like `HasCondition` for checking specific conditions in the game.

### LevelProgress

This class represents the progress of a specific level in the game. It contains fields for the level name, player position, arrays of Vector3 and Quaternion for storing positions and rotations of objects in the level, and an object called `LevelObjects` for storing various objects in the level (pickups, weapons, enemies, etc.).

### SerializedVector3 and SerializedQuaternion

These classes are used to serialize Unity's Vector3 and Quaternion data types for saving and loading purposes.

### LevelObjects

This class is used within `LevelProgress` to store information about different objects in a level. It contains arrays for pickups, swappable weapons, enemies, and physics objects. Each array is accompanied by corresponding arrays of positions and rotations.

### InventoryData

This class represents individual items in the player's inventory. It includes fields for item name, stackability, quantity, description, and more.

## Examples

Here's an example of how to use the game data save and load system:

```csharp
// Create an instance of SaveData and populate it with game data
SaveData saveData = new SaveData();
saveData.saveName = "MySave";
saveData.hour = 12;
saveData.day = 1;
saveData.month = 5;
saveData.year = 2023;

// Serialize the SaveData object to JSON
string jsonData = JsonUtility.ToJson(saveData);

// Save the JSON data to a file or player prefs

// Load the JSON data from a file or player prefs

// Deserialize the JSON data to a SaveData object
SaveData loadedSaveData = JsonUtility.FromJson<SaveData>(jsonData);

// Use the loadedSaveData object to retrieve saved game data
Debug.Log("Loaded Save Name: " + loadedSaveData.saveName);
Debug.Log("Loaded Hour: " + loadedSaveData.hour);
Debug.Log("Loaded Day: " + loadedSaveData.day);
```
