module EntitasTest
//let Generate content = 
//  File.WriteAllText (Path.Combine(__SOURCE_DIRECTORY__, "..\MyProject\Exceptions.cs"), content)

open System    
open System.IO
open NUnit.Framework
open Bosco.ECS
open ShmupWarz

[<TestFixture>]
type Tests() =

    let world = new World(64)

    let mutable e0:Entity = null
    let mutable e1:Entity = null
    let mutable e2:Entity = null
    let mutable e3:Entity = null

    //1st entity - no components
    [<Test>]
    member this.Test001() =

        e1 <- world.CreateEntity("Playerz")
            .AddBounds(1.0f)
            .AddHealth(100.0f, 100.0f)
            .IsPlayer(true)
            .AddResource("Fighter")

        Assert.AreEqual(1, e1.Id)
        Assert.AreEqual("Entity_Playerz(1)(Bounds,Health,Player,Resource)", e1.ToString())
        let m1 = Matcher.AllOf(1)
        let g1 = world.GetGroup(m1)
        //let s1 = g1.GetEntities()
        //Assert.AreEqual(1, s1.Length)
        Assert.AreEqual(1,1)

    [<Test>]//2nd entity - position
    member this.Test002() =

        e2 <- world.CreateEntity("Test1")
        e2.AddPosition(1.0f, 1.0f, 1.0f) |> ignore
        Assert.AreEqual("Entity_Test1(2)(Position)", e2.ToString())


    [<Test>]//3rd entity - position, Velocity
    member this.Test003() =

        let mutable k = 0
        e3 <- world.CreateEntity("Test2")
        let s = e3.OnComponentAdded.Subscribe(fun o -> 
            k <- k+1
            if k = 1 then
                Assert.AreEqual("Entity_Test2(3)(Position)", e3.ToString())
            else
                Assert.AreEqual("Entity_Test2(3)(Position,Velocity)", e3.ToString())
        )
        e3.AddPosition(1.0f, 1.0f, 1.0f) |> ignore
        e3.AddVelocity(0.1f, 0.1f, 1.0f) |> ignore

    [<Test>]//check total counts
    member this.Test004() =


        Assert.AreEqual(3, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(0, world.reusableEntitiesCount)

    [<Test>]//1 component matcher
    member this.Test005() =

        let m1 = Matcher.AllOf(10)
        Assert.AreEqual(true, m1.Matches(e2))

        let m2 = Matcher.AllOf(10,14)
        Assert.AreEqual(false, m2.Matches(e2))

    [<Test>]//2 component matcher
    member this.Test006() =

        let m1 = Matcher.AllOf(10)
        Assert.AreEqual(true, m1.Matches(e3))
        Assert.AreEqual("AllOf(10)", m1.ToString())

        let m2 = Matcher.AllOf(10,14)
        Assert.AreEqual(true, m2.Matches(e3))
        Assert.AreEqual("AllOf(10,14)", m2.ToString())


    [<Test>]//Groups
    member this.Test007() =

        let m2 = Matcher.AllOf(10)
        let g2 = world.GetGroup(m2)
        let s2 = g2.GetEntities()
        Assert.AreEqual(2, s2.Length)

        let g = world.GetGroup(Matcher.AllOf(10,14))
        let s = g.GetEntities()
        Assert.AreEqual(1, s.Length)

    [<Test>]//Groups
    member this.Test008() =

        e3.OnComponentRemoved.Subscribe(fun o ->
            Assert.AreEqual("Entity_Test2(3)(Velocity)", e3.ToString())
        ) |> ignore
        e3.RemovePosition()

    [<Test>]//Groups
    member this.Test009() =

        e0 <- world.CreateEntity("Hello")
        e0.AddHealth(10.0f, 10.0f) |> ignore
        Assert.AreEqual(4, e0.Id)
        Assert.AreEqual("Entity_Hello(4)(Health)", e0.ToString())

        let g7 = world.GetGroup(Matcher.AllOf(7))
        let s7 = g7.GetEntities()
        Assert.AreEqual(2, s7.Length)

        world.OnEntityDestroyed.Subscribe(fun o ->
            Assert.AreEqual(3, world.count)
            Assert.AreEqual(0, world.retainedEntitiesCount)
            Assert.AreEqual(0, world.reusableEntitiesCount)
        ) |> ignore

        world.DestroyEntity(e0)
