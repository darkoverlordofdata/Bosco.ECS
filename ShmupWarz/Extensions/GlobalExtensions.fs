namespace ShmupWarz
(**
 * Entitas Global Functions for ShmupWarz
 *
 * do not edit this file
 *)
[<AutoOpen>]
module GlobalFunctions =

    let isNull x = match x with null -> true | _ -> false
    let notNull x = match x with null -> false | _ -> true
    let rnd = new System.Random()
    let IsNull x = (** Unity Version *)
        let y = box x // In case of value types
        obj.ReferenceEquals(y, Unchecked.defaultof<_>) || // Regular null check
        y.Equals(Unchecked.defaultof<_>) // Will call Unity overload if needed
