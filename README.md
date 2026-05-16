# 2-Do-List

> *The tasks start simple. They don't stay that way.*
<img width="1080" height="280" alt="Screenshot 2026-05-16 135815" src="https://github.com/user-attachments/assets/9bdb3c3d-4d4d-461c-ad02-0a3adefffe19" />  

<br/>
<br/>

**2-Do-List** is a top-down narrative adventure game built in **Godot 4.6** with **C#**. You play as an unnamed office worker who wakes up each day with a to-do list to complete before he can move on. At first the tasks are mundane; check emails, grab the boss a coffee, help a coworker. But as the days pass, the list grows stranger, the world gets harder to explain, and it starts to feel like something bigger is going on beneath the surface.

---

## Gameplay

Explore a pixel art world one day at a time. Move through the environment, interact with NPCs and objects, and check off the tasks on your to-do list to advance. Choices you make along the way ripple forward in ways you might not expect.

**Controls**

| Action | Key |
|---|---|
| Move | `WASD` / Arrow Keys |
| Interact | `E` |
| Toggle To-Do List | `T` |
| Sprint | `Shift` |
| Fullscreen | `Alt+Enter` / `F11` |

---

## Story

Your first day on the job is already going sideways. Phone calls to make and records to update, a boss with a very specific coffee order, a coworker who's happy to let you take the fall for their mistakes, and a shareholder meeting that definitely did **not** get rescheduled. Every decision has consequences.

And that's just Day 1.

As the days progress, the to-do list evolves from ordinary errands into something more sinister. The game rewards curiosity, attentiveness, and the willingness to do (or not do) what must be done.

---

## Technical Highlights

### Narrative Scripting with Ink
Story, dialogue, and branching choices are authored in [**Ink**](https://www.inklestudios.com/ink/)- a professional narrative scripting language integrated into Godot via the [**GodotInk**](https://github.com/paulloz/godot-ink) addon. Ink handles all dialogue trees, conditional logic, and story state, keeping narrative content completely decoupled from engine code. This separation lets the story scale independently of the game systems.

### C# in Godot 4
The project is written primarily in **C#** using Godot 4's .NET integration. This project includes scene management, signal handling, and all custom game logic demonstrating practical use of a statically-typed language in a game engine context.

### Autoloaded Service Architecture
Core systems are structured as **autoloaded singletons**- globally accessible services that persist across scene changes:

| Singleton | Role |
|---|---|
| `GameManager` | Central game state and flow coordination |
| `SaveManager` | Persistent save/load system |
| `FullScreenToggle` | Display management utility |
| `DebugKey` | Development tooling |

This pattern keeps individual scenes separate from eachother, with the game built on top of the dialogue system, rather than tangled within it. 

### Save System
Persistence is handled by `SaveManager` using a **group-based discovery pattern**: any node in the `Persist` group is automatically found and serialized at save time. This data-driven approach means new persistent objects can be added to the world without touching save system code.

### Group-Based Interaction System
Interactables, the player, and persistent objects are organized via Godot's **global groups** (`interactable`, `player`, `Persist`). Game systems communicate through groups rather than direct node references, keeping scenes modular and easy to extend.


---
## Status  
  
**Work in Progress** - The current build contains the first level (Day 1) with a makeshift final cutscene included for demo purposes. 2-Do List was made as a capstone project at GCU (Grand Canyon University) and was developed over the course of the Spring 2026 semeseter. The demo version of the game that was produced was fully playable at the time of the capstone and the intended point in development we hoped to reach before the capstone presentation.

---

## Built With

- [Godot 4.6](https://godotengine.org/) - Game engine
- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/) - Primary scripting language
- [Ink](https://www.inklestudios.com/ink/) - Narrative scripting language
- [GodotInk](https://github.com/paulloz/godot-ink) - Ink runtime integration addon

---

## Authors

**Noah Addink**  
[github.com/NAddink](https://github.com/NAddink)  

**Briana Jamerson**  
[github.com/jambri12](https://github.com/jambri12)
