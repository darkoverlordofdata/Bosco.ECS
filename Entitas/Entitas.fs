(*
 * Entitas ECS
 *)
namespace Entitas

open System
open System.Text
open System.Collections.Generic

(**
  * Base Component Type
  *)
[<AbstractClass>][<AllowNullLiteral>]
type Component() = class end

(**
  * Entity Type
  *)
//[<AbstractClass>][<AllowNullLiteral>]
//type AEntity() =

//  abstract member Id:int with get,set
//  abstract member IsEnabled:bool with get,set
//  abstract member AddComponent: int * Component -> AEntity
//  abstract member RemoveComponent: int -> AEntity
//  abstract member ReplaceComponent: int * Component -> AEntity
//  abstract member GetComponent: int -> Component
// abstract member GetComponents: unit -> Component[]
//  abstract member HasComponent: int -> bool
//  abstract member HasComponents: int[] -> bool
//  abstract member HasAnyComponent: int[] -> bool
//  abstract member RemoveAllComponents: unit -> unit
//  abstract member Retain: Object -> unit
//  abstract member Release: Object -> unit

(**
  * Interface: System with an Initialization phase
  * Initialize is called before the game loop is started
  *)
type IInitializeSystem =
  abstract member Initialize: unit -> unit

(**
  * Interface: System with an Execute
  * Execute is called once per game loop
  *)
type IExecuteSystem =
  abstract member Execute: unit -> unit

