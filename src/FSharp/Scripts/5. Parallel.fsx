#load "SetupThespian.fsx"
#load @"..\GeoCoder.fs"

open System
open System.Diagnostics
open MBrace.Core
open MBrace.Thespian
open MBrace.Library.Cloud

let cluster = createLocalCluster(4)

// 1. You can run simple tasks in parallel across the cluster using Balanced
let balanced =
    [ 1 .. 20 ]
    |> Balanced.map(fun i -> i * i) // Execute this function on a worker
    |> cluster.Run

// 2. For tasks that have more complex distribution (or pipelines) you can use the generalised
// Cloud.Parallel operator.
let tasks = [ for i in 1 .. 20 -> cloud { return i * i } ]

let results =
    tasks
    |> Cloud.Parallel // go from (array of Cloud<int>) to (Cloud<array of int>)
    |> cluster.Run


// 3. Convert all of these lat / longs into addresses.

let locations =
    [ 51.556021, -0.279519
      52.516275, 13.377704
      38.897676, -77.036530
      52.525084, 13.369402 ]
    |> Seq.map (fun x -> cloud { 
           let lat, lon = x
           return GeoCoding.lookupAddress("AIzaSyAf-uZ-6xCoXdrFUZOYhNS-9cy0qFMDZYg", lat, lon) 
       })
    |> Cloud.Parallel
    |> cluster.Run
// You can use the GeoCoding module to lookup an address e.g.
// GeoCoding.lookupAddress(googleKey, 12.0, -40.0)
// You can use Balanced.map or Parallel, whichever you prefer.