namespace ShmupWarz
(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)
open System
open System.Collections.Generic
open Entitas
open ShmupWarz
open UnityEngine

[<AbstractClass>]
type CollisionPair(world:World, group1:Group, group2:Group) =

    member this.CheckForCollisions() =
        for a in group1.GetEntities() do
            for b in group2.GetEntities() do
                if this.CollisionExists(a, b) then this.HandleCollision(a, b)

    member this.CollisionExists(e1, e2) =
        let position1 = e1.position
        let position2 = e2.position

        let a = float(position1.x) - float(position2.x)
        let b = float(position1.y) - float(position2.y)
        (float32(Math.Sqrt(a * a + b * b)) - e1.bounds.radius) < e2.bounds.radius

    abstract member HandleCollision: Entity * Entity -> unit


type EnemyBulletCollision(world, bullets, enemies) =
    inherit CollisionPair(world, bullets, enemies)

    override this.HandleCollision(bullet, ship) =
        let pos = bullet.position
        world.CreateSmallExplosion(pos.x, pos.y) |> ignore
        //Shrapnel.Instance.Hit(pos.x, pos.y)
        bullet.IsDestroy(true) |> ignore

        let mutable health = ship.health
        health.health <- health.health-1.0f
        if health.health <= 0.0f then
            //world.score.value <- world.score.value + health.maximumHealth
            ship.IsDestroy(true) |> ignore
            let position = ship.position
            world.CreateBigExplosion(position.x, position.y) |> ignore
        else
            let percentage = Math.Truncate(float(health.health / health.maximumHealth) * 100.0)
            let text = ((ship.view).gameObject:?>GameObject).GetComponent("TextMesh")

            (text:?>TextMesh).text <- (sprintf "%i%%" (int percentage))


type PlayerMineCollision(world, mines, players) =
    inherit CollisionPair(world, mines, players)

    override this.HandleCollision(mine, player) =
        ()


type CollisionSystem(world:World) =

    let bullets = world.GetGroup(Matcher.Bullet)
    let enemies = world.GetGroup(Matcher.Enemy)
    let players = world.GetGroup(Matcher.Player)
    let mines = world.GetGroup(Matcher.Mine)
    let collisionPairs =
        [|
            new EnemyBulletCollision(world, bullets, enemies)
            new PlayerMineCollision(world, mines, players)
        |]:CollisionPair[]

    let playerSprites = Resources.LoadAll("fighterz")


    interface IExecuteSystem with
        member this.Execute() =
            for pair in collisionPairs do
                pair.CheckForCollisions()

    interface IInitializeSystem with
        member this.Initialize() =
            world.SetStatus(100.0f, 0.0f) |> ignore


