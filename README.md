# FeloxGame README

## Major features:
- Procedural terrain system
- Dynamic UI tree
- Graphics programming
	- Fragment and vertex shader programming
	- Sprite batching

## Commit messages
- ✨ New Feature
- ⚙️ Progress/tweaks
- 🐛 Bug fix
- 🧹 Code rework/refactor
- 📖 Other (e.g. documentation)

## What is this project?
- This is my first game project, building from a helpful GameEngine.TK project and the OpenTK learn documentation online.

## What am I trying to achieve?
I'm using this to improve my programming experience, but I've been interested in procedural terrain generation for years.

### __Coding Practices & Concepts Learned Through This Project__:
*recursion | enums | perlin noise | class inheritance | matrices | GLSL*
- **Constructors**: I've found a use of multiple constructors to enable setting default values for non compile-time constants. I've strengthened my understanding of the syntax of subclass constructors.
- **enums**: Finding opportunities to use them
- **Events**: Learning about subscribing to events to send data between uncoupled classes.
- **File handling**: Loading appropriately and efficiently from files, using JSON, ImageSharp, etc.
- **Github**: Learning the essentials of Git and the programmer workflow.
- **GLSL (shaders)**: Understanding the fundamentals of what a GPU should do vs a CPU; using uniforms; changing shaders; setting active texture units; studying the graphics pipeline
- **Inheritance**: Added class inheritance in Inventory.cs and strengthened my understandings of `base`
- **Matrix manipulation**: Use and understanding of vectors and matrices to draw the world, player, and UI.
- **Namespaces**: Organising code into a structured system; Knowing when to group files, but also when not to
- **[NonSerialized]**: Use of the NonSerialized tag for parts of an object I don't want saved to JSON 
- **The `out` keyword**: Finding practical applications for this concept in Inventory.cs and WorldManager.cs
- **Overloading**: Multiple instances of methods depending on where I'm using them (see WorldManager.cs for an example)
- **Polymorphic Serialisation**: Using JSON attributes to serialise subclasses of a base class separately.
- **Procedural world generation**: Use of noise functions to generate an infinite world
- **Recursion**: I use recursive methods to update and draw my UI system
- **Reference types**: Using references to objects to enable other options to access their methods without owning that object (e.g., the player is referenced by the World's entity draw call, but is external to that entity list).
- **Tree structures**: I have developed a tree structure to draw all UI objects within their parent objects