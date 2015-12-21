﻿namespace ShmupWarz
open System
open Entitas
open UnityEngine

type GameController () =
    inherit MonoBehaviour ()

    let createSystems(world:World) =
        world.Add(new MovementSystem(world))
        world.Add(new PlayerInputSystem(world))
        //world.Add(new SoundEffectSystem(world))
        world.Add(new CollisionSystem(world))
        world.Add(new ExpiringSystem(world))
        world.Add(new EntitySpawningTimerSystem(world))
        //world.Add(new ParallaxStarRepeatingSystem(world))
        //world.Add(new ColorAnimationSystem(world))
        world.Add(new ScaleAnimationSystem(world))
        world.Add(new RemoveOffscreenShipsSystem(world))
        world.Add(new RenderPositionSystem(world))
        world.Add(new AddViewSystem(world))
        //world.Add(new HealthRenderSystem(world))
        //world.Add(new HudRenderSystem(world))
        world.Add(new DestroySystem(world))

    let world = new World(Component.TotalComponents)

    (** *)
    member this.Start () =
        Debug.Log("Hello - ShmupWarz!")
        createSystems(world)
        world.Initialize()
        world.CreatePlayer()

    (** *)
    member this.Update () =
        world.Execute()


