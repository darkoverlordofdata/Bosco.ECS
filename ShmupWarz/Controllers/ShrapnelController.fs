namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine

type ShrapnelController () =
    inherit MonoBehaviour ()

    (** particles: Set Value in Instector *)
    [<DefaultValue>][<SerializeField>]
    val mutable particles:ParticleSystem

    [<DefaultValue>]
    static val mutable private _instance:ShrapnelController
    static member Instance with get() = ShrapnelController._instance

    let create(prefab, position) =
        let newParticleSystem  = Object.Instantiate(prefab, position, Quaternion.identity):?>ParticleSystem
        Object.Destroy(newParticleSystem.gameObject, newParticleSystem.startLifetime)
        newParticleSystem


    member this.Awake() = 
        //if not(Object.ReferenceEquals(ShrapnelController._instance, null)) then 
        //    failwith "Multiple instances of Shrapnel Provider!"
        ShrapnelController._instance = this

    member this.Hit(x, y) =
        let position = new Vector3(x, y, 0.0f)
        create(this.particles, position)