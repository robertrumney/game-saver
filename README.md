# SaveGame Script

The "SaveGame" script is a Unity component that enables saving and loading game data. It allows players to store their progress and resume gameplay from where they left off. This script utilizes binary serialization to save the game data to files and provides methods for managing save slots and capturing screenshots during the save process.

## Features

- Save and load game data.
- Support for multiple save slots.
- Retrieve skin tone and hair color from the saved data.
- Capture screenshots during the save process.

## Getting Started

To use the "SaveGame" script in your Unity project, follow these steps:

1. Attach the script to a GameObject in your scene.
2. Ensure that the necessary Unity libraries are imported.
3. Create an instance of the "SaveData" class to hold the game data.
4. Call the appropriate methods provided by the "SaveGame" script to save and load the data.

## Usage

### Setting Save Name

```csharp
SaveGame.instance.SetSaveName("MySave");
```
### Saving Game Data

```csharp
SaveGame.instance.Write();
```

### Loading Game Data

```csharp
SaveGame.instance.Read();
```

### Retrieving Colors

```csharp
Color skinTone = SaveGame.instance.SkinTone();
Color hairColor = SaveGame.instance.HairColor();
```
