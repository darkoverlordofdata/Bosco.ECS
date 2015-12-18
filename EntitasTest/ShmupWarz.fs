namespace ShmupWarz

    open Entitas
    open System
    open System.Collections.Generic
    open Microsoft.FSharp.Reflection

    type BoundsComponent() =
        inherit Component()
        member val radius = 0.0f with get, set

    type BulletComponent() =
        inherit Component()
        member val active = false with get, set

    type ColorAnimationComponent() =
        inherit Component()
        member val redMin = 0.0f with get, set
        member val redMax = 0.0f with get, set
        member val redSpeed = 0.0f with get, set
        member val greenMin = 0.0f with get, set
        member val greenMax = 0.0f with get, set
        member val greenSpeed = 0.0f with get, set
        member val blueMin = 0.0f with get, set
        member val blueMax = 0.0f with get, set
        member val blueSpeed = 0.0f with get, set
        member val alphaMin = 0.0f with get, set
        member val alphaMax = 0.0f with get, set
        member val alphaSpeed = 0.0f with get, set
        member val redAnimate = false with get, set
        member val greenAnimate = false with get, set
        member val blueAnimate = false with get, set
        member val alphaAnimate = false with get, set
        member val repeat = false with get, set

    type EnemyComponent() =
        inherit Component()
        member val active = false with get, set

    type ExpiresComponent() =
        inherit Component()
        member val delay = 0.0f with get, set

    type FiringComponent() =
        inherit Component()
        member val active = false with get, set

    type HealthComponent() =
        inherit Component()
        member val health = 0.0f with get, set
        member val maximumHealth = 0.0f with get, set

    type ParallaxStarComponent() =
        inherit Component()
        member val active = false with get, set

    type PlayerComponent() =
        inherit Component()
        member val active = false with get, set

    type PositionComponent() =
        inherit Component()
        member val x = 0.0f with get, set
        member val y = 0.0f with get, set

    type ScaleAnimationComponent() =
        inherit Component()
        member val min = 0.0f with get, set
        member val max = 0.0f with get, set
        member val speed = 0.0f with get, set
        member val repeat = false with get, set
        member val active = false with get, set

    type SoundEffectComponent() =
        inherit Component()
        member val effect = 0.0f with get, set

    type SpriteComponent() =
        inherit Component()
        member val layer = 0.0f with get, set
        member val game_object = null with get, set

    type VelocityComponent() =
        inherit Component()
        member val x = 0.0f with get, set
        member val y = 0.0f with get, set

    type ScoreComponent() =
        inherit Component()
        member val value = 0.0f with get, set

    type DestroyComponent() =
        inherit Component()
        member val active = false with get, set

    type MouseComponent() =
        inherit Component()
        member val x = 0.0f with get, set
        member val y = 0.0f with get, set

    type ScaleComponent() =
        inherit Component()
        member val x = 0.0f with get, set
        member val y = 0.0f with get, set

    type ResourceComponent() =
        inherit Component()
        member val name = "" with get, set

    type LayerComponent() =
        inherit Component()
        member val ordinal = 0.0f with get, set

    type BackgroundComponent() =
        inherit Component()
        member val filter = null with get, set

    type MineComponent() =
        inherit Component()
        member val active = false with get, set

    type StatusComponent() =
        inherit Component()
        member val percent = 0.0f with get, set
        member val immunity = 0.0f with get, set

    type LifeComponent() =
        inherit Component()
        member val count = 0.0f with get, set

    type ComponentIds = 
      | BoundsComponent of BoundsComponent
      | BulletComponent of BulletComponent
      | ColorAnimationComponent of ColorAnimationComponent
      | EnemyComponent of EnemyComponent
      | ExpiresComponent of ExpiresComponent
      | FiringComponent of FiringComponent
      | HealthComponent of HealthComponent
      | ParallaxStarComponent of ParallaxStarComponent
      | PlayerComponent of PlayerComponent
      | PositionComponent of PositionComponent
      | ScaleAnimationComponent of ScaleAnimationComponent
      | SoundEffectComponent of SoundEffectComponent
      | SpriteComponent of SpriteComponent
      | VelocityComponent of VelocityComponent
      | ScoreComponent of ScoreComponent
      | DestroyComponent of DestroyComponent
      | MouseComponent of MouseComponent
      | ScaleComponent of ScaleComponent
      | ResourceComponent of ResourceComponent
      | LayerComponent of LayerComponent
      | BackgroundComponent of BackgroundComponent
      | MineComponent of MineComponent
      | StatusComponent of StatusComponent
      | LifeComponent of LifeComponent

    type Globals() =
        member this.Components 
            with get() = FSharpType.GetUnionCases typeof<ComponentIds>  

    type MovementSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type PlayerInputSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()
        interface IInitializeSystem with
            member this.Initialize() =
                ()

    type SoundEffectSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()
        interface IInitializeSystem with
            member this.Initialize() =
                ()

    type CollisionSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()
        interface IInitializeSystem with
            member this.Initialize() =
                ()

    type ExpiringSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type EntitySpawningTimerSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()
        interface IInitializeSystem with
            member this.Initialize() =
                ()

    type ParallaxStarRepeatingSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()
        interface IInitializeSystem with
            member this.Initialize() =
                ()

    type ColorAnimationSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type ScaleAnimationSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type RemoveOffscreenShipsSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type SpriteRenderSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type HealthRenderSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type HudRenderSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()
        interface IInitializeSystem with
            member this.Initialize() =
                ()

    type DestroySystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()

    type AddViewSystem(world) =
        class end

    type BackgroundSystem(world) =
        interface IExecuteSystem with
            member this.Execute() =
                ()
        interface IInitializeSystem with
            member this.Initialize() =
                ()
