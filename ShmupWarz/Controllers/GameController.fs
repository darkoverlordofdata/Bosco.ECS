namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
(**
 * GameController
 *
 * Main Game Loop
 * Attached to Assets/Scenes/Game Scene/Game Controller
 *)
type GameController () =
    inherit MonoBehaviour ()

    let world = new World(Component.TotalComponents)

    let createSystems(world:World) =
        world.Add(new MovementSystem(world))
        world.Add(new PlayerInputSystem(world))
        //world.Add(new SoundEffectSystem(world)) Replaced in prefabs
        world.Add(new CollisionSystem(world))
        world.Add(new ExpiringSystem(world))
        world.Add(new EntitySpawningTimerSystem(world))
        //world.Add(new ParallaxStarRepeatingSystem(world)) Repaced with static background
        //world.Add(new ColorAnimationSystem(world)) Replaced with particles (ShrapnelController)
        world.Add(new ScaleAnimationSystem(world))
        world.Add(new RemoveOffscreenShipsSystem(world))
        world.Add(new RenderPositionSystem(world))
        world.Add(new ViewManagerSystem(world))
        world.Add(new DestroySystem(world))

    (** *)
    member this.Start () =
        createSystems(world)
        world.Initialize()
        world.CreatePlayer()

    (** *)
    member this.Update () =
        world.Execute()


