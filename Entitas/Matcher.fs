namespace Entitas

open System
open System.Text
open System.Collections.Generic

(** 
 * Matcher
 * matchers can match an entity by components used
 *)
type Matcher () =
  static let mutable uniqueId           = 0
  do uniqueId <- uniqueId+1

  let _id                               = uniqueId
  let mutable _indices                  = Array.empty
  let mutable _allOfIndices             = Array.empty
  let mutable _anyOfIndices             = Array.empty
  let mutable _noneOfIndices            = Array.empty
  let mutable toStringCache             = ""

  static let toStringHelper(sb:StringBuilder, text:string, indices:int[]) =
    if indices.Length > 0 then
      sb.Append(text+"(") |> ignore
      for i=0 to indices.Length-1 do
        sb.Append(indices.[i].ToString()) |> ignore
        if i < indices.Length-1 then sb.Append(",") |> ignore
      sb.Append(")") |> ignore

  member val uuid = System.Guid.NewGuid().ToString() with get

  member this.Id
    with get() = _id
  member this.Indices
    with get():int[] = 
      if _indices.Length = 0 then
        _indices <- this.mergeIndices()
      _indices
  member internal this.AllOfIndices
    with get() = _allOfIndices
    and  set(value) = _allOfIndices <- value
  member internal this.AnyOfIndices
    with get() = _anyOfIndices
    and  set(value) = _anyOfIndices <- value
  member internal this.NoneOfIndices
    with get() = _noneOfIndices
    and  set(value) = _noneOfIndices <- value

  (** 
   * AnyOf 
   *
   * @param indices
   * @returns this 
   *)
  member this.AnyOf([<ParamArray>] indices: int[]) =
    _anyOfIndices <- Matcher.distinctIndices(_indices)
    _indices <- Array.empty
    this

  member this.AnyOf([<ParamArray>] matchers: Matcher[]) =
    this.AnyOf(Matcher.mergeIndices(matchers))

  (** 
   * NoneOf 
   *
   * @param indices
   * @returns this 
   *)
  member this.NoneOf([<ParamArray>] indices: int[]) =
    _noneOfIndices <- Matcher.distinctIndices(_indices)
    _indices <- Array.empty
    this

  member this.NoneOf([<ParamArray>] matchers: Matcher[]) =
    this.NoneOf(Matcher.mergeIndices(matchers))
   
  member this.mergeIndices():int[] =
    let indicesList = new ResizeArray<int>()
    indicesList.AddRange(_allOfIndices)
    indicesList.AddRange(_anyOfIndices)
    indicesList.AddRange(_noneOfIndices)
    _indices <- Matcher.distinctIndices(indicesList.ToArray())
    _indices

  (** 
   * Matches 
   *
   * @param entity
   * @returns true if entity is a match 
   *)
  member this.Matches(entity:Entity) =
    let matchesAllOf = if _allOfIndices.Length = 0 then true else entity.HasComponents(_allOfIndices)
    let matchesAnyOf = if _anyOfIndices.Length = 0 then true else entity.HasAnyComponent(_allOfIndices)
    let matchesNoneOf = if _noneOfIndices.Length = 0 then true else not(entity.HasAnyComponent(_allOfIndices))
    matchesAllOf && matchesAnyOf && matchesNoneOf



  (** 
   * AllOf 
   *
   * @param indices
   * @returns this 
   *)
  static member AllOf([<ParamArray>] indices: int[]) =
    let matcher = new Matcher()
    matcher.AllOfIndices <- Matcher.distinctIndices(indices)
    matcher

  static member AllOf([<ParamArray>] matchers: Matcher[]) =
    Matcher.AllOf(Matcher.mergeIndices(matchers))

  (** 
   * AnyOf 
   *
   * @param indices
   * @returns this 
   *)
  static member AnyOf([<ParamArray>] indices: int[]) =
    let matcher = new Matcher()
    matcher.AnyOfIndices <- Matcher.distinctIndices(indices)
    matcher

  static member AnyOf([<ParamArray>] matchers: Matcher[]) =
    Matcher.AnyOf(Matcher.mergeIndices(matchers))

  (** 
   * mergeIndicse 
   *
   * @param matchers
   * @returns array of indices for the matchers
   *)
  static member mergeIndices(matchers:Matcher[]):int[] =
    let mutable indices = (Array.zeroCreate matchers.Length)
    for i=0 to matchers.Length-1 do
      let matcher = matchers.[i]
      if matcher.Indices.Length <> 1 then
        failwithf "Matcher indices length not = 1 %s" (matchers.[i].ToString())
      indices.[i] <- matcher.Indices.[0]
    indices

  (** 
   * distinctIndicse 
   *
   * @param indices
   * @returns array of indices with duplicates removed
   *)
  static member distinctIndices(indices:int[]):int[] = 
    let indicesSet = new HashSet<int>(indices)
    let mutable uniqueIndices = (Array.zeroCreate indicesSet.Count)
    indicesSet.CopyTo(uniqueIndices)
    Array.Sort(uniqueIndices)
    uniqueIndices

  (** 
   * ToString
   *
   * @returns the string representation of this matcher
   *)
  override this.ToString() =


    if toStringCache = "" then

      let sb = new StringBuilder()
      toStringHelper(sb, "AllOf", _allOfIndices)
      toStringHelper(sb, "AnyOf", _anyOfIndices)
      toStringHelper(sb, "NoneOf", _noneOfIndices)
      toStringCache <- sb.ToString()

    toStringCache


  