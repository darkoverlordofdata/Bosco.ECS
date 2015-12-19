module WorldExtensions

open Entitas
open System
open System.Collections.Generic
open UnityEngine
open ShmupWarz
open ShmupWarzExtensions

let rnd = new System.Random()

type World with

    
    member this.CreatePlayer() =
        let pos = Camera.main.ScreenToWorldPoint(new Vector3(float32(Screen.width/2), 100.0f, 0.0f))
        this.CreateEntity()
            .AddBounds(1.0f)
            .AddHealth(100.0f, 100.0f)
            .AddPosition(pos.x, pos.y, pos.z)
            .IsPlayer(true)
            .AddResource("Fighter")


    member this.CreateBullet(x, y) =
        this.CreateEntity()
            .AddBounds(0.1f)
            .AddVelocity(0.0f, float32(800*3), 0.0f)
            .AddPosition(x, y, 0.0f)
            .AddExpires(2.0f)
            .AddSoundEffect(0.0f)
            .IsBullet(true)
            .AddResource("Bullet")


    member this.CreateMine(health, x, y, velocity) =
        let pos = Camera.main.ScreenToWorldPoint(new Vector3(x, float32(Screen.height)-y, 0.0f))
        this.CreateEntity()
            .AddBounds(0.25f)
            .AddVelocity(0.0f, -velocity, 0.0f)
            .AddPosition(pos.x, pos.y, pos.z)
            .AddHealth(health*10.0f, health*10.0f)
            .IsMine(true)
            .AddResource("Mine"+(int health).ToString())


    member this.CreateLife(ordinal) =
        let x = float32((Screen.width/2)-((ordinal+1) * 40)+87)
        let y = 80
        let pos = Camera.main.ScreenToWorldPoint(new Vector3(x, float32(Screen.height-y), 0.0f))
        this.CreateEntity()
            .AddPosition(pos.x, pos.y, pos.z)
            .AddLife(float32(ordinal))
            .AddResource("Life")


    member this.CreateStatus() =
        let x = float32(Screen.width/2)
        let y = 120
        
        let pos = Camera.main.ScreenToWorldPoint(new Vector3(x, float32(Screen.height-y), 0.0f))

        this.SetStatus(100.0f, 0.0f)
            .AddPosition(pos.x, pos.y, pos.z)
            .AddResource("Status")

    member this.CreateEnemy1() =
        let x = float32(rnd.Next(Screen.width))
        let y = float32(Screen.height-100)
        
        let pos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0.0f))
        
        this.CreateEntity()
            .AddBounds(1.0f)
            .AddPosition(pos.x, pos.y, pos.z)
            .AddVelocity(0.0f, float32(-40*3), 0.0f)
            .AddHealth(10.0f, 10.0f)
            .IsEnemy(true)
            .AddResource("Enemy1")

    member this.CreateEnemy2() =
        let x = float32(rnd.Next(Screen.width))
        let y = float32(Screen.height-200)
        
        let pos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0.0f))
        
        this.CreateEntity()
            .AddBounds(2.0f)
            .AddPosition(pos.x, pos.y, pos.z)
            .AddVelocity(0.0f, float32(-30*3), 0.0f)
            .AddHealth(20.0f, 20.0f)
            .IsEnemy(true)
            .AddResource("Enemy2")

    member this.CreateEnemy3() =
        let x = float32(rnd.Next(Screen.width))
        let y = float32(Screen.height-300)
        
        let pos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0.0f))
        
        this.CreateEntity()
            .AddBounds(3.0f)
            .AddPosition(pos.x, pos.y, pos.z)
            .AddVelocity(0.0f, float32(-20*3), 0.0f)
            .AddHealth(40.0f, 40.0f)
            .IsEnemy(true)
            .AddResource("Enemy3")


    member this.CreateHugeExplosion(x, y) =
        let scale = 1.0f
        this.CreateEntity()
            .AddExpires(0.5f)
            .AddScale(scale, scale)
            .AddScaleAnimation(scale/100.0f, scale, -3.0f, false, true)
            .AddPosition(x, y, 00.f)
            .AddResource("BigExplosion")

    member this.CreateBigExplosion(x, y) =
        let scale = 0.5f
        this.CreateEntity()
            .AddExpires(0.5f)
            .AddScale(scale, scale)
            .AddScaleAnimation(scale/100.0f, scale, -3.0f, false, true)
            .AddPosition(x, y, 00.f)
            .AddResource("BigExplosion")

    member this.CreateSmallExplosion(x, y) =
        let scale = 0.1f
        this.CreateEntity()
            .AddExpires(0.5f)
            .AddScale(scale, scale)
            .AddScaleAnimation(scale/100.0f, scale, -3.0f, false, true)
            .AddPosition(x, y, 00.f)
            .AddResource("SmallExplosion")


