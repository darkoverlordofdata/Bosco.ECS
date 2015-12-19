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
type Component() = 
    static member None with get() = 0

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

