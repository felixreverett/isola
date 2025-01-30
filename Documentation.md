# FeloxGame Documentation

## 1. Inventory Systems
- Inventories exist as an object property of an entity such as the Player and chests.
- The PlayerInventory is unique in that it has an additional mouseSlot.
- When an inventory is accessed, it is handled by a corresponding user interface.
	- Opening the player's inventory depicts only the player inventory
	- Opening a chest depicts both the player inventory and chest inventory together.

## 2. Entities

### 2.1. Entity.cs
- Position: Vector2 to denote word position around origin.

### 2.2. TileEntity.cs
- Position: Vector2 which represents the lower-left corner of the tile the TileEntity is on
- DrawPositionOffset: Offsets the draw position of the tile as required