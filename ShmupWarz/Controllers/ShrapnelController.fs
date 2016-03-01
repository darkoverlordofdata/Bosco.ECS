namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
(**
 * ShrapnelController
 *
 * Adapter to a Unity3D Particle Generator
 * Attached to Assets/Scenes/Game Scene/ShrapnelController
 *)
type ShrapnelController () =
    inherit MonoBehaviour ()

    (** particles: Set Value in Unity3D Instector *)
    [<DefaultValue>][<SerializeField>]
    val mutable particles:ParticleSystem

    (** save the instance reference when we wake up *)
    [<DefaultValue>]static val mutable private instance:ShrapnelController
    member this.Awake() = ShrapnelController.instance <- this

    (** 
     * Spawn shrapnel particles
     *)
    static member Spawn(x, y) =
        let position = new Vector3(x, y, 0.0f)
        let newParticleSystem  = Object.Instantiate(ShrapnelController.instance.particles, position, Quaternion.identity):?>ParticleSystem
        Object.Destroy(newParticleSystem.gameObject, newParticleSystem.startLifetime)
        newParticleSystem



