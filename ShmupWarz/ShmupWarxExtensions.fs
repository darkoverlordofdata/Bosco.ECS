module ShmupWarzExtensions

open Entitas
open System
open System.Collections.Generic
open Microsoft.FSharp.Reflection

let isNull x = match x with null -> true | _ -> false

type ComponentIds = 
  | BoundsComponent = 1
  | BulletComponent = 2
  | ColorAnimationComponent = 3
  | EnemyComponent = 4
  | ExpiresComponent = 5
  | FiringComponent = 6
  | HealthComponent = 7
  | ParallaxStarComponent = 8
  | PlayerComponent = 9
  | PositionComponent = 10
  | ScaleAnimationComponent = 11
  | SoundEffectComponent = 12
  | SpriteComponent = 13
  | VelocityComponent = 14
  | ScoreComponent = 15
  | DestroyComponent = 16
  | MouseComponent = 17
  | ScaleComponent = 18
  | ResourceComponent = 19
  | LayerComponent = 20
  | BackgroundComponent = 21
  | MineComponent = 22
  | StatusComponent = 23
  | LifeComponent = 24

type Entity with
    member this.bounds

        with get() = this.GetComponent(int ComponentId.Bounds):?>BoundsComponent

    member this.hasBounds
        with get() = this.HasComponent(int ComponentId.Bounds)

    member this._boundsComponentPool
         with get() = new Stack<BoundsComponent>()

    member this.ClearBoundsComponentPool() =
        this._boundsComponentPool.Clear()

    member this.AddBounds(radius) =
        let mutable c = 
          match this._boundsComponentPool.Count with
          | 0 -> new BoundsComponent()
          | _ -> this._boundsComponentPool.Pop()
        c.radius = radius;
        this.AddComponent(int ComponentId.Bounds, c)

    member this.ReplaceBounds(radius) =
        let previousComponent = if this.hasbounds then this.bounds else null
        let mutable c = 
          match this._boundsComponentPool.Count with
          | 0 -> new BoundsComponent()
          | _ -> this._boundsComponentPool.Pop()
        c.radius = radius;
        this.ReplaceComponent(int ComponentId.Bounds, c) |> ignore
        if not(isNull(previousComponent)) then
            this._boundsComponentPool.Push(previousComponent)
        this

    member this.RemoveBounds() =
        let c = this.bounds
        this.RemoveComponent(int ComponentId.Bounds) |> ignore
        this._boundsComponentPool.Push(c)

type Matcher with 
    static member Bounds with get() = Matcher.AllOf(int ComponentId.Bounds) 

type Entity with

    static member bulletComponent= new BulletComponent()

    member this.isBullet
        with get() =
            this.HasComponent(int ComponentId.Bullet)
        and  set(value) =
            if value <> this.isBullet then
                this.AddComponent(int ComponentId.Bullet, Entity.bulletComponent) |> ignore
            else
                this.RemoveComponent(int ComponentId.Bullet) |> ignore

    member this.IsBullet(value) =
        this.isBullet <- value
        this

type Matcher with 
    static member Bullet with get() = Matcher.AllOf(int ComponentId.Bullet) 

type Entity with
    member this.colorAnimation

        with get() = this.GetComponent(int ComponentId.ColorAnimation):?>ColorAnimationComponent

    member this.hasColorAnimation
        with get() = this.HasComponent(int ComponentId.ColorAnimation)

    member this._colorAnimationComponentPool
         with get() = new Stack<ColorAnimationComponent>()

    member this.ClearColorAnimationComponentPool() =
        this._colorAnimationComponentPool.Clear()

    member this.AddColorAnimation(redMin, redMax, redSpeed, greenMin, greenMax, greenSpeed, blueMin, blueMax, blueSpeed, alphaMin, alphaMax, alphaSpeed, redAnimate, greenAnimate, blueAnimate, alphaAnimate, repeat) =
        let mutable c = 
          match this._colorAnimationComponentPool.Count with
          | 0 -> new ColorAnimationComponent()
          | _ -> this._colorAnimationComponentPool.Pop()
        c.redMin = redMin;
        c.redMax = redMax;
        c.redSpeed = redSpeed;
        c.greenMin = greenMin;
        c.greenMax = greenMax;
        c.greenSpeed = greenSpeed;
        c.blueMin = blueMin;
        c.blueMax = blueMax;
        c.blueSpeed = blueSpeed;
        c.alphaMin = alphaMin;
        c.alphaMax = alphaMax;
        c.alphaSpeed = alphaSpeed;
        c.redAnimate = redAnimate;
        c.greenAnimate = greenAnimate;
        c.blueAnimate = blueAnimate;
        c.alphaAnimate = alphaAnimate;
        c.repeat = repeat;
        this.AddComponent(int ComponentId.ColorAnimation, c)

    member this.ReplaceColorAnimation(redMin, redMax, redSpeed, greenMin, greenMax, greenSpeed, blueMin, blueMax, blueSpeed, alphaMin, alphaMax, alphaSpeed, redAnimate, greenAnimate, blueAnimate, alphaAnimate, repeat) =
        let previousComponent = if this.hascolorAnimation then this.colorAnimation else null
        let mutable c = 
          match this._colorAnimationComponentPool.Count with
          | 0 -> new ColorAnimationComponent()
          | _ -> this._colorAnimationComponentPool.Pop()
        c.redMin = redMin;
        c.redMax = redMax;
        c.redSpeed = redSpeed;
        c.greenMin = greenMin;
        c.greenMax = greenMax;
        c.greenSpeed = greenSpeed;
        c.blueMin = blueMin;
        c.blueMax = blueMax;
        c.blueSpeed = blueSpeed;
        c.alphaMin = alphaMin;
        c.alphaMax = alphaMax;
        c.alphaSpeed = alphaSpeed;
        c.redAnimate = redAnimate;
        c.greenAnimate = greenAnimate;
        c.blueAnimate = blueAnimate;
        c.alphaAnimate = alphaAnimate;
        c.repeat = repeat;
        this.ReplaceComponent(int ComponentId.ColorAnimation, c) |> ignore
        if not(isNull(previousComponent)) then
            this._colorAnimationComponentPool.Push(previousComponent)
        this

    member this.RemoveColorAnimation() =
        let c = this.colorAnimation
        this.RemoveComponent(int ComponentId.ColorAnimation) |> ignore
        this._colorAnimationComponentPool.Push(c)

type Matcher with 
    static member ColorAnimation with get() = Matcher.AllOf(int ComponentId.ColorAnimation) 

type Entity with

    static member enemyComponent= new EnemyComponent()

    member this.isEnemy
        with get() =
            this.HasComponent(int ComponentId.Enemy)
        and  set(value) =
            if value <> this.isEnemy then
                this.AddComponent(int ComponentId.Enemy, Entity.enemyComponent) |> ignore
            else
                this.RemoveComponent(int ComponentId.Enemy) |> ignore

    member this.IsEnemy(value) =
        this.isEnemy <- value
        this

type Matcher with 
    static member Enemy with get() = Matcher.AllOf(int ComponentId.Enemy) 

type Entity with
    member this.expires

        with get() = this.GetComponent(int ComponentId.Expires):?>ExpiresComponent

    member this.hasExpires
        with get() = this.HasComponent(int ComponentId.Expires)

    member this._expiresComponentPool
         with get() = new Stack<ExpiresComponent>()

    member this.ClearExpiresComponentPool() =
        this._expiresComponentPool.Clear()

    member this.AddExpires(delay) =
        let mutable c = 
          match this._expiresComponentPool.Count with
          | 0 -> new ExpiresComponent()
          | _ -> this._expiresComponentPool.Pop()
        c.delay = delay;
        this.AddComponent(int ComponentId.Expires, c)

    member this.ReplaceExpires(delay) =
        let previousComponent = if this.hasexpires then this.expires else null
        let mutable c = 
          match this._expiresComponentPool.Count with
          | 0 -> new ExpiresComponent()
          | _ -> this._expiresComponentPool.Pop()
        c.delay = delay;
        this.ReplaceComponent(int ComponentId.Expires, c) |> ignore
        if not(isNull(previousComponent)) then
            this._expiresComponentPool.Push(previousComponent)
        this

    member this.RemoveExpires() =
        let c = this.expires
        this.RemoveComponent(int ComponentId.Expires) |> ignore
        this._expiresComponentPool.Push(c)

type Matcher with 
    static member Expires with get() = Matcher.AllOf(int ComponentId.Expires) 

type Entity with

    static member firingComponent= new FiringComponent()

    member this.isFiring
        with get() =
            this.HasComponent(int ComponentId.Firing)
        and  set(value) =
            if value <> this.isFiring then
                this.AddComponent(int ComponentId.Firing, Entity.firingComponent) |> ignore
            else
                this.RemoveComponent(int ComponentId.Firing) |> ignore

    member this.IsFiring(value) =
        this.isFiring <- value
        this

type Matcher with 
    static member Firing with get() = Matcher.AllOf(int ComponentId.Firing) 

type Entity with
    member this.health

        with get() = this.GetComponent(int ComponentId.Health):?>HealthComponent

    member this.hasHealth
        with get() = this.HasComponent(int ComponentId.Health)

    member this._healthComponentPool
         with get() = new Stack<HealthComponent>()

    member this.ClearHealthComponentPool() =
        this._healthComponentPool.Clear()

    member this.AddHealth(health, maximumHealth) =
        let mutable c = 
          match this._healthComponentPool.Count with
          | 0 -> new HealthComponent()
          | _ -> this._healthComponentPool.Pop()
        c.health = health;
        c.maximumHealth = maximumHealth;
        this.AddComponent(int ComponentId.Health, c)

    member this.ReplaceHealth(health, maximumHealth) =
        let previousComponent = if this.hashealth then this.health else null
        let mutable c = 
          match this._healthComponentPool.Count with
          | 0 -> new HealthComponent()
          | _ -> this._healthComponentPool.Pop()
        c.health = health;
        c.maximumHealth = maximumHealth;
        this.ReplaceComponent(int ComponentId.Health, c) |> ignore
        if not(isNull(previousComponent)) then
            this._healthComponentPool.Push(previousComponent)
        this

    member this.RemoveHealth() =
        let c = this.health
        this.RemoveComponent(int ComponentId.Health) |> ignore
        this._healthComponentPool.Push(c)

type Matcher with 
    static member Health with get() = Matcher.AllOf(int ComponentId.Health) 

type Entity with

    static member parallaxStarComponent= new ParallaxStarComponent()

    member this.isParallaxStar
        with get() =
            this.HasComponent(int ComponentId.ParallaxStar)
        and  set(value) =
            if value <> this.isParallaxStar then
                this.AddComponent(int ComponentId.ParallaxStar, Entity.parallaxStarComponent) |> ignore
            else
                this.RemoveComponent(int ComponentId.ParallaxStar) |> ignore

    member this.IsParallaxStar(value) =
        this.isParallaxStar <- value
        this

type Matcher with 
    static member ParallaxStar with get() = Matcher.AllOf(int ComponentId.ParallaxStar) 

type Entity with

    static member playerComponent= new PlayerComponent()

    member this.isPlayer
        with get() =
            this.HasComponent(int ComponentId.Player)
        and  set(value) =
            if value <> this.isPlayer then
                this.AddComponent(int ComponentId.Player, Entity.playerComponent) |> ignore
            else
                this.RemoveComponent(int ComponentId.Player) |> ignore

    member this.IsPlayer(value) =
        this.isPlayer <- value
        this

type Matcher with 
    static member Player with get() = Matcher.AllOf(int ComponentId.Player) 

type Entity with
    member this.position

        with get() = this.GetComponent(int ComponentId.Position):?>PositionComponent

    member this.hasPosition
        with get() = this.HasComponent(int ComponentId.Position)

    member this._positionComponentPool
         with get() = new Stack<PositionComponent>()

    member this.ClearPositionComponentPool() =
        this._positionComponentPool.Clear()

    member this.AddPosition(x, y) =
        let mutable c = 
          match this._positionComponentPool.Count with
          | 0 -> new PositionComponent()
          | _ -> this._positionComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.AddComponent(int ComponentId.Position, c)

    member this.ReplacePosition(x, y) =
        let previousComponent = if this.hasposition then this.position else null
        let mutable c = 
          match this._positionComponentPool.Count with
          | 0 -> new PositionComponent()
          | _ -> this._positionComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.ReplaceComponent(int ComponentId.Position, c) |> ignore
        if not(isNull(previousComponent)) then
            this._positionComponentPool.Push(previousComponent)
        this

    member this.RemovePosition() =
        let c = this.position
        this.RemoveComponent(int ComponentId.Position) |> ignore
        this._positionComponentPool.Push(c)

type Matcher with 
    static member Position with get() = Matcher.AllOf(int ComponentId.Position) 

type Entity with
    member this.scaleAnimation

        with get() = this.GetComponent(int ComponentId.ScaleAnimation):?>ScaleAnimationComponent

    member this.hasScaleAnimation
        with get() = this.HasComponent(int ComponentId.ScaleAnimation)

    member this._scaleAnimationComponentPool
         with get() = new Stack<ScaleAnimationComponent>()

    member this.ClearScaleAnimationComponentPool() =
        this._scaleAnimationComponentPool.Clear()

    member this.AddScaleAnimation(min, max, speed, repeat, active) =
        let mutable c = 
          match this._scaleAnimationComponentPool.Count with
          | 0 -> new ScaleAnimationComponent()
          | _ -> this._scaleAnimationComponentPool.Pop()
        c.min = min;
        c.max = max;
        c.speed = speed;
        c.repeat = repeat;
        c.active = active;
        this.AddComponent(int ComponentId.ScaleAnimation, c)

    member this.ReplaceScaleAnimation(min, max, speed, repeat, active) =
        let previousComponent = if this.hasscaleAnimation then this.scaleAnimation else null
        let mutable c = 
          match this._scaleAnimationComponentPool.Count with
          | 0 -> new ScaleAnimationComponent()
          | _ -> this._scaleAnimationComponentPool.Pop()
        c.min = min;
        c.max = max;
        c.speed = speed;
        c.repeat = repeat;
        c.active = active;
        this.ReplaceComponent(int ComponentId.ScaleAnimation, c) |> ignore
        if not(isNull(previousComponent)) then
            this._scaleAnimationComponentPool.Push(previousComponent)
        this

    member this.RemoveScaleAnimation() =
        let c = this.scaleAnimation
        this.RemoveComponent(int ComponentId.ScaleAnimation) |> ignore
        this._scaleAnimationComponentPool.Push(c)

type Matcher with 
    static member ScaleAnimation with get() = Matcher.AllOf(int ComponentId.ScaleAnimation) 

type Entity with
    member this.soundEffect

        with get() = this.GetComponent(int ComponentId.SoundEffect):?>SoundEffectComponent

    member this.hasSoundEffect
        with get() = this.HasComponent(int ComponentId.SoundEffect)

    member this._soundEffectComponentPool
         with get() = new Stack<SoundEffectComponent>()

    member this.ClearSoundEffectComponentPool() =
        this._soundEffectComponentPool.Clear()

    member this.AddSoundEffect(effect) =
        let mutable c = 
          match this._soundEffectComponentPool.Count with
          | 0 -> new SoundEffectComponent()
          | _ -> this._soundEffectComponentPool.Pop()
        c.effect = effect;
        this.AddComponent(int ComponentId.SoundEffect, c)

    member this.ReplaceSoundEffect(effect) =
        let previousComponent = if this.hassoundEffect then this.soundEffect else null
        let mutable c = 
          match this._soundEffectComponentPool.Count with
          | 0 -> new SoundEffectComponent()
          | _ -> this._soundEffectComponentPool.Pop()
        c.effect = effect;
        this.ReplaceComponent(int ComponentId.SoundEffect, c) |> ignore
        if not(isNull(previousComponent)) then
            this._soundEffectComponentPool.Push(previousComponent)
        this

    member this.RemoveSoundEffect() =
        let c = this.soundEffect
        this.RemoveComponent(int ComponentId.SoundEffect) |> ignore
        this._soundEffectComponentPool.Push(c)

type Matcher with 
    static member SoundEffect with get() = Matcher.AllOf(int ComponentId.SoundEffect) 

type Entity with
    member this.sprite

        with get() = this.GetComponent(int ComponentId.Sprite):?>SpriteComponent

    member this.hasSprite
        with get() = this.HasComponent(int ComponentId.Sprite)

    member this._spriteComponentPool
         with get() = new Stack<SpriteComponent>()

    member this.ClearSpriteComponentPool() =
        this._spriteComponentPool.Clear()

    member this.AddSprite(layer, object) =
        let mutable c = 
          match this._spriteComponentPool.Count with
          | 0 -> new SpriteComponent()
          | _ -> this._spriteComponentPool.Pop()
        c.layer = layer;
        c.object = object;
        this.AddComponent(int ComponentId.Sprite, c)

    member this.ReplaceSprite(layer, object) =
        let previousComponent = if this.hassprite then this.sprite else null
        let mutable c = 
          match this._spriteComponentPool.Count with
          | 0 -> new SpriteComponent()
          | _ -> this._spriteComponentPool.Pop()
        c.layer = layer;
        c.object = object;
        this.ReplaceComponent(int ComponentId.Sprite, c) |> ignore
        if not(isNull(previousComponent)) then
            this._spriteComponentPool.Push(previousComponent)
        this

    member this.RemoveSprite() =
        let c = this.sprite
        this.RemoveComponent(int ComponentId.Sprite) |> ignore
        this._spriteComponentPool.Push(c)

type Matcher with 
    static member Sprite with get() = Matcher.AllOf(int ComponentId.Sprite) 

type Entity with
    member this.velocity

        with get() = this.GetComponent(int ComponentId.Velocity):?>VelocityComponent

    member this.hasVelocity
        with get() = this.HasComponent(int ComponentId.Velocity)

    member this._velocityComponentPool
         with get() = new Stack<VelocityComponent>()

    member this.ClearVelocityComponentPool() =
        this._velocityComponentPool.Clear()

    member this.AddVelocity(x, y) =
        let mutable c = 
          match this._velocityComponentPool.Count with
          | 0 -> new VelocityComponent()
          | _ -> this._velocityComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.AddComponent(int ComponentId.Velocity, c)

    member this.ReplaceVelocity(x, y) =
        let previousComponent = if this.hasvelocity then this.velocity else null
        let mutable c = 
          match this._velocityComponentPool.Count with
          | 0 -> new VelocityComponent()
          | _ -> this._velocityComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.ReplaceComponent(int ComponentId.Velocity, c) |> ignore
        if not(isNull(previousComponent)) then
            this._velocityComponentPool.Push(previousComponent)
        this

    member this.RemoveVelocity() =
        let c = this.velocity
        this.RemoveComponent(int ComponentId.Velocity) |> ignore
        this._velocityComponentPool.Push(c)

type Matcher with 
    static member Velocity with get() = Matcher.AllOf(int ComponentId.Velocity) 

type Entity with
    member this.score

        with get() = this.GetComponent(int ComponentId.Score):?>ScoreComponent

    member this.hasScore
        with get() = this.HasComponent(int ComponentId.Score)

    member this._scoreComponentPool
         with get() = new Stack<ScoreComponent>()

    member this.ClearScoreComponentPool() =
        this._scoreComponentPool.Clear()

    member this.AddScore(value) =
        let mutable c = 
          match this._scoreComponentPool.Count with
          | 0 -> new ScoreComponent()
          | _ -> this._scoreComponentPool.Pop()
        c.value = value;
        this.AddComponent(int ComponentId.Score, c)

    member this.ReplaceScore(value) =
        let previousComponent = if this.hasscore then this.score else null
        let mutable c = 
          match this._scoreComponentPool.Count with
          | 0 -> new ScoreComponent()
          | _ -> this._scoreComponentPool.Pop()
        c.value = value;
        this.ReplaceComponent(int ComponentId.Score, c) |> ignore
        if not(isNull(previousComponent)) then
            this._scoreComponentPool.Push(previousComponent)
        this

    member this.RemoveScore() =
        let c = this.score
        this.RemoveComponent(int ComponentId.Score) |> ignore
        this._scoreComponentPool.Push(c)

type Matcher with 
    static member Score with get() = Matcher.AllOf(int ComponentId.Score) 

type Entity with

    static member destroyComponent= new DestroyComponent()

    member this.isDestroy
        with get() =
            this.HasComponent(int ComponentId.Destroy)
        and  set(value) =
            if value <> this.isDestroy then
                this.AddComponent(int ComponentId.Destroy, Entity.destroyComponent) |> ignore
            else
                this.RemoveComponent(int ComponentId.Destroy) |> ignore

    member this.IsDestroy(value) =
        this.isDestroy <- value
        this

type Matcher with 
    static member Destroy with get() = Matcher.AllOf(int ComponentId.Destroy) 

type Entity with
    member this.mouse

        with get() = this.GetComponent(int ComponentId.Mouse):?>MouseComponent

    member this.hasMouse
        with get() = this.HasComponent(int ComponentId.Mouse)

    member this._mouseComponentPool
         with get() = new Stack<MouseComponent>()

    member this.ClearMouseComponentPool() =
        this._mouseComponentPool.Clear()

    member this.AddMouse(x, y) =
        let mutable c = 
          match this._mouseComponentPool.Count with
          | 0 -> new MouseComponent()
          | _ -> this._mouseComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.AddComponent(int ComponentId.Mouse, c)

    member this.ReplaceMouse(x, y) =
        let previousComponent = if this.hasmouse then this.mouse else null
        let mutable c = 
          match this._mouseComponentPool.Count with
          | 0 -> new MouseComponent()
          | _ -> this._mouseComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.ReplaceComponent(int ComponentId.Mouse, c) |> ignore
        if not(isNull(previousComponent)) then
            this._mouseComponentPool.Push(previousComponent)
        this

    member this.RemoveMouse() =
        let c = this.mouse
        this.RemoveComponent(int ComponentId.Mouse) |> ignore
        this._mouseComponentPool.Push(c)

type Matcher with 
    static member Mouse with get() = Matcher.AllOf(int ComponentId.Mouse) 

type Entity with
    member this.scale

        with get() = this.GetComponent(int ComponentId.Scale):?>ScaleComponent

    member this.hasScale
        with get() = this.HasComponent(int ComponentId.Scale)

    member this._scaleComponentPool
         with get() = new Stack<ScaleComponent>()

    member this.ClearScaleComponentPool() =
        this._scaleComponentPool.Clear()

    member this.AddScale(x, y) =
        let mutable c = 
          match this._scaleComponentPool.Count with
          | 0 -> new ScaleComponent()
          | _ -> this._scaleComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.AddComponent(int ComponentId.Scale, c)

    member this.ReplaceScale(x, y) =
        let previousComponent = if this.hasscale then this.scale else null
        let mutable c = 
          match this._scaleComponentPool.Count with
          | 0 -> new ScaleComponent()
          | _ -> this._scaleComponentPool.Pop()
        c.x = x;
        c.y = y;
        this.ReplaceComponent(int ComponentId.Scale, c) |> ignore
        if not(isNull(previousComponent)) then
            this._scaleComponentPool.Push(previousComponent)
        this

    member this.RemoveScale() =
        let c = this.scale
        this.RemoveComponent(int ComponentId.Scale) |> ignore
        this._scaleComponentPool.Push(c)

type Matcher with 
    static member Scale with get() = Matcher.AllOf(int ComponentId.Scale) 

type Entity with
    member this.resource

        with get() = this.GetComponent(int ComponentId.Resource):?>ResourceComponent

    member this.hasResource
        with get() = this.HasComponent(int ComponentId.Resource)

    member this._resourceComponentPool
         with get() = new Stack<ResourceComponent>()

    member this.ClearResourceComponentPool() =
        this._resourceComponentPool.Clear()

    member this.AddResource(name) =
        let mutable c = 
          match this._resourceComponentPool.Count with
          | 0 -> new ResourceComponent()
          | _ -> this._resourceComponentPool.Pop()
        c.name = name;
        this.AddComponent(int ComponentId.Resource, c)

    member this.ReplaceResource(name) =
        let previousComponent = if this.hasresource then this.resource else null
        let mutable c = 
          match this._resourceComponentPool.Count with
          | 0 -> new ResourceComponent()
          | _ -> this._resourceComponentPool.Pop()
        c.name = name;
        this.ReplaceComponent(int ComponentId.Resource, c) |> ignore
        if not(isNull(previousComponent)) then
            this._resourceComponentPool.Push(previousComponent)
        this

    member this.RemoveResource() =
        let c = this.resource
        this.RemoveComponent(int ComponentId.Resource) |> ignore
        this._resourceComponentPool.Push(c)

type Matcher with 
    static member Resource with get() = Matcher.AllOf(int ComponentId.Resource) 

type Entity with
    member this.layer

        with get() = this.GetComponent(int ComponentId.Layer):?>LayerComponent

    member this.hasLayer
        with get() = this.HasComponent(int ComponentId.Layer)

    member this._layerComponentPool
         with get() = new Stack<LayerComponent>()

    member this.ClearLayerComponentPool() =
        this._layerComponentPool.Clear()

    member this.AddLayer(ordinal) =
        let mutable c = 
          match this._layerComponentPool.Count with
          | 0 -> new LayerComponent()
          | _ -> this._layerComponentPool.Pop()
        c.ordinal = ordinal;
        this.AddComponent(int ComponentId.Layer, c)

    member this.ReplaceLayer(ordinal) =
        let previousComponent = if this.haslayer then this.layer else null
        let mutable c = 
          match this._layerComponentPool.Count with
          | 0 -> new LayerComponent()
          | _ -> this._layerComponentPool.Pop()
        c.ordinal = ordinal;
        this.ReplaceComponent(int ComponentId.Layer, c) |> ignore
        if not(isNull(previousComponent)) then
            this._layerComponentPool.Push(previousComponent)
        this

    member this.RemoveLayer() =
        let c = this.layer
        this.RemoveComponent(int ComponentId.Layer) |> ignore
        this._layerComponentPool.Push(c)

type Matcher with 
    static member Layer with get() = Matcher.AllOf(int ComponentId.Layer) 

type Entity with
    member this.background

        with get() = this.GetComponent(int ComponentId.Background):?>BackgroundComponent

    member this.hasBackground
        with get() = this.HasComponent(int ComponentId.Background)

    member this._backgroundComponentPool
         with get() = new Stack<BackgroundComponent>()

    member this.ClearBackgroundComponentPool() =
        this._backgroundComponentPool.Clear()

    member this.AddBackground(filter) =
        let mutable c = 
          match this._backgroundComponentPool.Count with
          | 0 -> new BackgroundComponent()
          | _ -> this._backgroundComponentPool.Pop()
        c.filter = filter;
        this.AddComponent(int ComponentId.Background, c)

    member this.ReplaceBackground(filter) =
        let previousComponent = if this.hasbackground then this.background else null
        let mutable c = 
          match this._backgroundComponentPool.Count with
          | 0 -> new BackgroundComponent()
          | _ -> this._backgroundComponentPool.Pop()
        c.filter = filter;
        this.ReplaceComponent(int ComponentId.Background, c) |> ignore
        if not(isNull(previousComponent)) then
            this._backgroundComponentPool.Push(previousComponent)
        this

    member this.RemoveBackground() =
        let c = this.background
        this.RemoveComponent(int ComponentId.Background) |> ignore
        this._backgroundComponentPool.Push(c)

type Matcher with 
    static member Background with get() = Matcher.AllOf(int ComponentId.Background) 

type Entity with

    static member mineComponent= new MineComponent()

    member this.isMine
        with get() =
            this.HasComponent(int ComponentId.Mine)
        and  set(value) =
            if value <> this.isMine then
                this.AddComponent(int ComponentId.Mine, Entity.mineComponent) |> ignore
            else
                this.RemoveComponent(int ComponentId.Mine) |> ignore

    member this.IsMine(value) =
        this.isMine <- value
        this

type Matcher with 
    static member Mine with get() = Matcher.AllOf(int ComponentId.Mine) 

type Entity with
    member this.status

        with get() = this.GetComponent(int ComponentId.Status):?>StatusComponent

    member this.hasStatus
        with get() = this.HasComponent(int ComponentId.Status)

    member this._statusComponentPool
         with get() = new Stack<StatusComponent>()

    member this.ClearStatusComponentPool() =
        this._statusComponentPool.Clear()

    member this.AddStatus(percent, immunity) =
        let mutable c = 
          match this._statusComponentPool.Count with
          | 0 -> new StatusComponent()
          | _ -> this._statusComponentPool.Pop()
        c.percent = percent;
        c.immunity = immunity;
        this.AddComponent(int ComponentId.Status, c)

    member this.ReplaceStatus(percent, immunity) =
        let previousComponent = if this.hasstatus then this.status else null
        let mutable c = 
          match this._statusComponentPool.Count with
          | 0 -> new StatusComponent()
          | _ -> this._statusComponentPool.Pop()
        c.percent = percent;
        c.immunity = immunity;
        this.ReplaceComponent(int ComponentId.Status, c) |> ignore
        if not(isNull(previousComponent)) then
            this._statusComponentPool.Push(previousComponent)
        this

    member this.RemoveStatus() =
        let c = this.status
        this.RemoveComponent(int ComponentId.Status) |> ignore
        this._statusComponentPool.Push(c)

type Matcher with 
    static member Status with get() = Matcher.AllOf(int ComponentId.Status) 

type Entity with
    member this.life

        with get() = this.GetComponent(int ComponentId.Life):?>LifeComponent

    member this.hasLife
        with get() = this.HasComponent(int ComponentId.Life)

    member this._lifeComponentPool
         with get() = new Stack<LifeComponent>()

    member this.ClearLifeComponentPool() =
        this._lifeComponentPool.Clear()

    member this.AddLife(count) =
        let mutable c = 
          match this._lifeComponentPool.Count with
          | 0 -> new LifeComponent()
          | _ -> this._lifeComponentPool.Pop()
        c.count = count;
        this.AddComponent(int ComponentId.Life, c)

    member this.ReplaceLife(count) =
        let previousComponent = if this.haslife then this.life else null
        let mutable c = 
          match this._lifeComponentPool.Count with
          | 0 -> new LifeComponent()
          | _ -> this._lifeComponentPool.Pop()
        c.count = count;
        this.ReplaceComponent(int ComponentId.Life, c) |> ignore
        if not(isNull(previousComponent)) then
            this._lifeComponentPool.Push(previousComponent)
        this

    member this.RemoveLife() =
        let c = this.life
        this.RemoveComponent(int ComponentId.Life) |> ignore
        this._lifeComponentPool.Push(c)

type Matcher with 
    static member Life with get() = Matcher.AllOf(int ComponentId.Life) 

