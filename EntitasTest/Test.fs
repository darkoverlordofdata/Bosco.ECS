module EntitasTest

open System    
open NUnit.Framework
open Entitas
open Microsoft.FSharp.Reflection
open ShmupWarz
open ShmupWarzExtensions

[<TestFixture>]
type FirstTest() =

    let world = new World(64)

    let CreatePlayer() =
        let e = world.CreateEntity()
        e.AddPosition(1.0f, 1.0f) |> ignore
        e.AddBounds(0.75f) |> ignore
        e.IsBullet(true) |> ignore
        e.AddHealth(200.0f, 200.0f) |> ignore
        e.AddVelocity(0.1f, 0.1f) |> ignore
        e



    let mutable e1:Entity = null
    let mutable e2:Entity = null
    let mutable e3:Entity = null

    [<Test>]//1st entity - no components
    member this.TestId1() =

        e1 <- CreatePlayer()
        let id = e1.Id
        Assert.AreEqual(1, id)


    [<Test>]//2nd entity - name
    member this.TestId2() =

        e2 <- world.CreateEntity()
        e2.AddPosition(1.0f, 1.0f) |> ignore
        Assert.AreEqual(e2.ToString(), "Entity_2(0)(Position)")


    [<Test>]//3rd entity - name, property
    member this.TestId3() =

        let mutable k = 0
        e3 <- world.CreateEntity()
        let s = e3.OnComponentAdded.Subscribe(fun o -> 
            k <- k+1
            if k = 1 then
                Assert.AreEqual("Entity_3(0)(Position)", e3.ToString())
            else
                Assert.AreEqual("Entity_3(0)(Position,Velocity)", e3.ToString())
        )
        e3.AddPosition(1.0f, 1.0f) |> ignore
        e3.AddVelocity(0.1f, 0.1f) |> ignore

    [<Test>]//check total counts
    member this.TestId4() =

        Assert.AreEqual(3, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(0, world.reusableEntitiesCount)

    [<Test>]//1 component matcher
    member this.TestId5() =

        let m1 = Matcher.AllOf(10)
        Assert.AreEqual(true, m1.Matches(e2))

        let m2 = Matcher.AllOf(10,14)
        Assert.AreEqual(false, m2.Matches(e2))

    [<Test>]//2 component matcher
    member this.TestId6() =

        let m1 = Matcher.AllOf(10)
        Assert.AreEqual(true, m1.Matches(e3))
        Assert.AreEqual("AllOf(10)", m1.ToString())

        let m2 = Matcher.AllOf(10,14)
        Assert.AreEqual(true, m2.Matches(e3))
        Assert.AreEqual("AllOf(10,14)", m2.ToString())


    [<Test>]//Groups
    member this.TestId7() =

        let g2 = world.GetGroup(Matcher.AllOf(10))
        let s2 = g2.GetEntities()
        Assert.AreEqual(3, s2.Length)

        let g = world.GetGroup(Matcher.AllOf(10,14))
        let s = g.GetEntities()
        Assert.AreEqual(2, s.Length)


    [<Test>]//Systems
    member this.TestId8() =

        let s = new CollisionSystem()
        world.Add(s)
        world.Initialize()
        world.Execute()
