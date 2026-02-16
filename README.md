# Tactical Drone Commander

A **Tower Defense / RTS hybrid** game built with Unity, demonstrating professional architecture patterns, modern C# practices, and scalable system design.

**Tech Stack:** Unity 2022.3+ | C# 11.0 | .NET Standard 2.1

## Overview

**Tactical Drone Commander** is a wave-based defense game where players command drones to protect their base from enemy attacks. The project showcases professional game development practices including Clean Architecture, Dependency Injection, Event-Driven Design, and performance optimization.

### Key Features

- **Wave-based gameplay** with progressive difficulty scaling
- **Strategic drone management** with RTS-style controls
- **Dynamic AI targeting** system with weighted decision making
- **Upgrade system** for player progression
- **Save/Load functionality** with persistent high scores
- **Responsive UI** with real-time health bars and game state feedback

---

## Architecture

This project demonstrates **production-ready architecture** with clear separation of concerns and adherence to SOLID principles.

### Project Structure

```
Assets/Scripts/
├── Core/                      # Core game systems
│   ├── GameStateMachine.cs   # FSM implementation with State Pattern
│   ├── GameBootstrapper.cs   # Game initialization & lifecycle
│   ├── GameInstaller.cs      # Dependency Injection configuration
│   ├── MainLifetimeScope.cs  # VContainer root scope
│   ├── Events/               # Event-Driven Architecture
│   │   ├── EventBus.cs       # Custom type-safe event bus
│   │   └── GameEvents.cs     # Typed event definitions
│   └── States/               # Game state implementations
│       ├── PregameState.cs
│       ├── WaveState.cs
│       ├── PostwaveState.cs
│       └── GameOverState.cs
│
├── Infrastructure/           # Cross-cutting concerns
│   ├── SaveLoadService.cs   # Persistent data management
│   ├── PoolService.cs       # Object pooling for performance
│   ├── InputController.cs   # Centralized input handling
│   └── AssetProvider.cs     # Resource loading abstraction
│
├── Systems/                  # Game logic systems (ECS-inspired)
│   ├── CombatSystem.cs      # Attack processing & damage calculation
│   ├── MovementSystem.cs    # Entity movement & NavMesh integration
│   ├── TargetingSystem.cs   # Target selection algorithms
│   └── RegenerationSystem.cs # Health regeneration logic
│
├── Entities/                 # Domain models
│   ├── Entity.cs            # Base entity class
│   ├── PlayerEntity.cs      # Player drone with upgrades
│   ├── EnemyEntity.cs       # Enemy drone
│   └── BaseEntity.cs        # Defensive structure
│
├── Controllers/              # MonoBehaviour bridges
│   ├── PlayerDroneController.cs
│   ├── EnemyController.cs
│   └── BaseController.cs
│
├── Gameplay/                 # Game-specific logic
│   ├── WaveManager.cs       # Wave progression system
│   ├── EntitiesManager.cs   # Entity registry & lookup
│   ├── EnemySpawner.cs      # Enemy instantiation
│   └── UpgradeSpawner.cs    # Power-up generation
│
└── UI/                       # User interface
    ├── MainMenu.cs
    ├── GameOver.cs
    └── HealthBarService.cs
```

### Design Patterns Used

| Pattern | Implementation | Purpose |
|---------|---------------|---------|
| **State Pattern** | `GameStateMachine`, `IGameState` | Manage game states (Pregame, Wave, Postwave, GameOver) |
| **Observer Pattern** | `EventBus`, typed events | Decoupled communication between systems |
| **Strategy Pattern** | Combat/Movement/Targeting Systems | Interchangeable algorithms for game logic |
| **Object Pool Pattern** | `PoolService` | Optimize instantiation of enemies and projectiles |
| **Dependency Injection** | VContainer | Inversion of Control and testability |
| **Factory Pattern** | Spawner classes | Centralized object creation |
| **Singleton** | Service classes via DI | Shared state management |

---

## Technical Stack

### Core Technologies
- **Unity 2022.3+** - Game engine
- **C# 11.0** - Programming language
- **.NET Standard 2.1** - Runtime

### Libraries & Packages
- **[VContainer](https://github.com/hadashiA/VContainer)** - Fast and lightweight DI container
- **[UniTask](https://github.com/Cysharp/UniTask)** - Efficient async/await for Unity
- **[DOTween](http://dotween.demigiant.com/)** - Animation and tweening
- **[Newtonsoft.Json](https://www.newtonsoft.com/json)** - JSON serialization
- **Unity NavMesh** - AI pathfinding
- **New Input System** - Modern input handling
- **TextMeshPro** - Advanced text rendering

---

## Key Technical Highlights

### 1. Finite State Machine (FSM)

The game uses a robust FSM implementation with the State Pattern to manage different game phases (Pregame, Wave, Postwave, GameOver). Each state is encapsulated in its own class implementing a common interface, allowing clean transitions with validation. The state machine validates transitions before executing them, ensuring the game can't enter invalid states. When a state change occurs, the system publishes events through the EventBus, allowing other systems to react without tight coupling.

### 2. Event-Driven Architecture

Built a custom type-safe EventBus that uses C# generics to ensure compile-time type checking for all events. The system allows any component to subscribe to specific event types and receive notifications when those events are published. This decouples systems completely - for example, the WaveManager doesn't need to know about the UI system, it just publishes a WaveCompletedEvent, and any interested systems can react. All events implement a common IGameEvent interface for type safety.

### 3. Dependency Injection with VContainer

All game services are registered through a centralized installer that configures the DI container. Services are created with Singleton lifetime to ensure single instances throughout the game. The container handles dependency resolution automatically - when a class needs a service, it's injected through the constructor. This makes the codebase highly testable and maintainable, as dependencies are explicit and can be easily mocked or replaced.

### 4. Async/Await with UniTask

All asynchronous operations use UniTask instead of coroutines for better performance and cleaner syntax. Every async operation receives a CancellationToken, allowing proper cleanup when objects are destroyed or operations need to be cancelled. For example, enemy AI loops run asynchronously, continuously updating targeting and movement, but immediately stop when the enemy dies or the game state changes. This prevents memory leaks and ensures resources are properly released.

### 5. ECS-Inspired System Architecture

Game logic is separated into stateless systems (Combat, Movement, Targeting, Regeneration) that operate on Entity data. Systems expose clean interfaces and contain no state themselves - all data lives in Entity objects. This makes the logic highly reusable and testable. For instance, the CombatSystem can process attacks between any entities without knowing whether they're players or enemies. Systems communicate results through the EventBus rather than direct references.

### 6. Object Pooling for Performance

Frequently instantiated objects (enemies, projectiles, VFX) are managed through a PoolService to eliminate garbage collection spikes. The pool pre-creates objects at startup and reuses them instead of instantiating/destroying. When an object is needed, the pool activates an existing one or creates a new one if the pool is exhausted. This dramatically reduces frame time spikes that would otherwise occur during intense combat with many simultaneous spawns.

---

## Getting Started

### Prerequisites
- Unity 6000.0.63f1 or newer
- Git LFS (for large assets)

### Installation

1. Clone the repository: `git clone https://github.com/kucJlbluJluMoH/TacticalDroneCommander.git`

2. Open in Unity Hub, click "Add" and select the project folder. Dependencies will auto-install via Package Manager.

3. Open the main scene at `Assets/Scenes/MainScene.unity` and press Play.

### Controls
- **Left Click** - Select drone
- **Left Click** - Move selected drone
---

## Learning Outcomes

This project demonstrates:

- **Clean Architecture** - Layered structure with clear dependencies  
- **SOLID Principles** - Single Responsibility, Open/Closed, Dependency Inversion  
- **Design Patterns** - State, Observer, Strategy, Object Pool, Factory  
- **Dependency Injection** - IoC container setup and usage  
- **Event-Driven Design** - Decoupled system communication  
- **Async Programming** - UniTask, CancellationToken, proper async patterns  
- **Performance Optimization** - Object pooling, profiling, GC optimization  
- **Modern Unity Practices** - New Input System, NavMesh, Addressables-ready  

---

## Performance Considerations

- **Object Pooling**: Enemies and projectiles are pooled to minimize GC allocations
- **Event System**: Dictionary-based event routing for O(1) lookups
- **NavMesh**: Efficient AI pathfinding with dynamic obstacle avoidance
- **DOTween**: Optimized tweening library for smooth animations
- **UniTask**: Zero-allocation async/await for Unity

---

## Future Improvements

- [ ] Add unit tests for core systems (FSM, EventBus, CombatSystem)
- [ ] Implement integration tests for gameplay flow
- [ ] Add performance benchmarks and profiling reports
- [ ] Create custom editor tools for level design
- [ ] Implement addressable assets for memory optimization
- [ ] Add localization support
- [ ] Implement replay system
- [ ] Add analytics integration

---

## Code Quality

### Naming Conventions
- **Interfaces**: `IServiceName` (e.g., `IEventBus`)
- **Private fields**: `_camelCase` (e.g., `_gameStateMachine`)
- **Public methods**: `PascalCase` (e.g., `SwitchState`)
- **Events**: `PascalCase` with "Event" suffix (e.g., `WaveCompletedEvent`)

### Architecture Principles
- **Separation of Concerns**: Each class has a single, well-defined responsibility
- **Dependency Inversion**: Depend on abstractions (interfaces), not concrete implementations
- **Open/Closed**: Systems are open for extension, closed for modification
- **Interface Segregation**: Small, focused interfaces rather than large, monolithic ones

---

## Author

**Ilia Nerovnov**
- GitHub: [@kucJlbluJluMoH](https://github.com/kucJlbluJluMoH)
- Email: ilyanerovnoff@gmail.com

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- Unity Technologies for the game engine
- VContainer by hadashiA for DI framework
- UniTask by Cysharp for async utilities
- DOTween by Demigiant for animation system

---

## References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Game Programming Patterns by Robert Nystrom](https://gameprogrammingpatterns.com/)
- [Unity Best Practices](https://unity.com/how-to/programming-unity)
- [VContainer Documentation](https://vcontainer.hadashikick.jp/)
- [UniTask Documentation](https://github.com/Cysharp/UniTask)

---

<p align="center">
  <i>Built with care using Unity</i>
</p>

