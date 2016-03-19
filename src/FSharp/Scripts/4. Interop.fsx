#load "SetupThespian.fsx"

open System
open System.Diagnostics
open MBrace.Core
open MBrace.Thespian

let cluster = createLocalCluster(4)

// 1. We can also call out to any .NET assembly within a cloud { }
let numberInFSharp = 10

let answerFromCSharp =
    cloud {
        let calc = CSharp.Calculator()
        return calc.Add(numberInFSharp, 5)
    } |> cluster.Run

// 2. One for you. Make a cloud computation to get the current geolocation. CSharp project
// has the geolocation code already.

// tip ... to go from a Task<T> to Cloud<T>, you need to use Cloud.AwaitTask
let location = cloud {
    let! latLong = 
        CSharp.GeoLocation.GetLongLat("AIzaSyAf-uZ-6xCoXdrFUZOYhNS-9cy0qFMDZYg") 
        |> Cloud.AwaitTask

    latLong |> printfn "%A"

    return latLong
} 
location |> cluster.Run



