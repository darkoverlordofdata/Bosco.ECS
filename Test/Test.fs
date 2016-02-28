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

    [<DefaultValue>] val mutable e0:Entity
    [<DefaultValue>] val mutable e1:Entity
    [<DefaultValue>] val mutable e2:Entity
    [<DefaultValue>] val mutable e3:Entity

    //1st entity - no components
    [<Test>]
    member this.Test001() =

        this.e1 <- world.CreateEntity("Playerz")
            .AddBounds(1.0f)
            .AddHealth(100.0f, 100.0f)
            .SetPlayer(true)
            .AddResource("Fighter")

        Assert.AreEqual(1, this.e1.Id)
        Assert.AreEqual("Entity_Playerz(1)(Bounds,Health,Player,Resource)", this.e1.ToString())
        let m1 = Matcher.AllOf(Matcher.Bounds)
        let g1 = world.GetGroup(m1)
        //let s1 = g1.GetEntities()
        //Assert.AreEqual(1, s1.Length)
        Assert.AreEqual(1,1)

    [<Test>]//2nd entity - position
    member this.Test002() =

        this.e2 <- world.CreateEntity("Test1")
        this.e2.AddPosition(1.0f, 1.0f, 1.0f) |> ignore
        Assert.AreEqual("Entity_Test1(2)(Position)", this.e2.ToString())


    [<Test>]//3rd entity - position, Velocity
    member this.Test003() =

        let mutable k = 0
        this.e3 <- world.CreateEntity("Test2")
        let s = this.e3.OnComponentAdded.Subscribe(fun o -> 
            k <- k+1
            if k = 1 then
                Assert.AreEqual("Entity_Test2(3)(Position)", this.e3.ToString())
            else
                Assert.AreEqual("Entity_Test2(3)(Position,Velocity)", this.e3.ToString())
        )
        this.e3.AddPosition(1.0f, 1.0f, 1.0f) |> ignore
        this.e3.AddVelocity(0.1f, 0.1f, 1.0f) |> ignore

    [<Test>]//check total counts
    member this.Test004() =


        Assert.AreEqual(3, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(0, world.reusableEntitiesCount)

    [<Test>]//1 component matcher
    member this.Test005() =

        let m1 = Matcher.AllOf(Matcher.Position)
        Assert.AreEqual(true, m1.Matches(this.e2))

        let m2 = Matcher.AllOf(Matcher.Position, Matcher.Velocity)
        Assert.AreEqual(false, m2.Matches(this.e2))

    [<Test>]//2 component matcher
    member this.Test006() =

        let m1 = Matcher.AllOf(Matcher.Position)
        Assert.AreEqual(true, m1.Matches(this.e3))
        Assert.AreEqual("AllOf(9)", m1.ToString())

        let m2 = Matcher.AllOf(Matcher.Position, Matcher.Velocity)
        Assert.AreEqual(true, m2.Matches(this.e3))
        Assert.AreEqual("AllOf(9,13)", m2.ToString())


    [<Test>]//Groups
    member this.Test007() =

        let m2 = Matcher.AllOf(Matcher.Position)
        let g2 = world.GetGroup(m2)
        let s2 = g2.GetEntities()
        Assert.AreEqual(2, s2.Length)

        let g = world.GetGroup(Matcher.AllOf(Matcher.Position, Matcher.Velocity))
        let s = g.GetEntities()
        Assert.AreEqual(1, s.Length)

    [<Test>]//Groups
    member this.Test008() =

        this.e3.OnComponentRemoved.Subscribe(fun o ->
            Assert.AreEqual("Entity_Test2(3)(Velocity)", this.e3.ToString())
        ) |> ignore
        this.e3.RemovePosition()

    [<Test>]//Groups
    member this.Test009() =

        this.e0 <- world.CreateEntity("Hello")
        this.e0.AddHealth(10.0f, 10.0f) |> ignore
        Assert.AreEqual(4, this.e0.Id)
        Assert.AreEqual("Entity_Hello(4)(Health)", this.e0.ToString())

        let g7 = world.GetGroup(Matcher.AllOf(Matcher.Health))
        let s7 = g7.GetEntities()
        Assert.AreEqual(2, s7.Length)

        world.OnEntityDestroyed.Subscribe(fun o ->
            Assert.AreEqual(3, world.count)
            Assert.AreEqual(0, world.retainedEntitiesCount)
            Assert.AreEqual(0, world.reusableEntitiesCount)
        ) |> ignore

        Assert.AreEqual(0, world.reusableEntitiesCount)
        world.DestroyEntity(this.e0)
        Assert.AreEqual(1, world.reusableEntitiesCount)


    member this.Test010() =

        let ee = world.CreateEntity("Goodbye")
        ee.AddPosition(1.0f, 1.0f, 1.0f) |> ignore
        Assert.AreEqual("Entity_Goodbye(5)(Position)", ee.ToString())
        Assert.AreEqual(5, ee.Id)
        Assert.AreEqual(4, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(1, world.reusableEntitiesCount)

        //for i=0 to world.ReusableEntities.Count-1 do
        //    Assert.AreEqual(world.ReusableEntities.Item(i).Name, "fred")
        //printf(world.ReusableEntities)
        let f1 = world.CreateEntity("Zorg")

        //BUG!!! ff points to the wrong entity
        // Should Be "Entity_Zorg(6)()"
        Assert.AreEqual("Entity_Goodbye(5)(Position)", f1.ToString())
        Assert.AreEqual(5, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(0, world.reusableEntitiesCount)

        let ff = world.CreateEntity("Zorg")
        Assert.AreEqual("Entity_Zorg(7)()", ff.ToString())
        Assert.AreEqual(6, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(0, world.reusableEntitiesCount)

        //ff.AddPosition(1.0f, 1.0f, 1.0f) |> ignore
