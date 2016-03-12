﻿#load "SetupThespian.fsx"
#load @"..\GeoCoder.fs"

open System
open MBrace.Core
open MBrace.Thespian
open MBrace.Library.Cloud
open System.IO
open System.Net
open MBrace.Flow

let cluster = createLocalCluster(4)

// CloudFlow is a distributed data flow library - think LINQ but scalable across machines.

// To get "into" a CloudFlow, use one of the following CloudFlow.Of* combinators e.g.
//CloudFlow.OfArray
//CloudFlow.OfCloudFileByLine (actually works for more than one file at a time)
//CloudFlow.OfHttpFileByLine

// Then perform any number of operations on the flow in sequence e.g.

//CloudFlow.map
//CloudFlow.filter
//CloudFlow.distinct
//CloudFlow.groupBy

// Then "exit" the CloudFlow by calling either a function that accumulates data into
// a single value, or starts with "to" e.g

//CloudFlow.toArray
//CloudFlow.toTextCloudFiles
//CloudFlow.sum
//CloudFlow.average
//CloudFlow.exists

// Finally we run the entire cloudflow against the cluster
// myCloudflow |> cluster.Run

// 1. Start by opening a handle to a book we already inserted into the cluster.

let huckleberryFinn = CloudFlow.OfCloudFileByLine("books/huckleberryfinn.txt")

// With this we can write distributed queries against the book.

// 2. Count the number of lines in the book
// All of the "combinators" live inside the CloudFlow module.
let lines =
    huckleberryFinn
    |> CloudFlow.length // accumulator - ends the flow
    |> cluster.Run

// 3. We can chain up CloudFlow calls, just like LINQ.
let linesLongerThanFiftyCharacters =
    huckleberryFinn
    |> CloudFlow.filter(fun line -> line.Length > 50) // each line called here
    |> CloudFlow.length
    |> cluster.Run

// 4. Convert the book to uppercase and get back the first 50 rows of the book.
// Use CloudFlow.map (Select in LINQ) to convert each line
// Then Take to restrict to 50 rows
// Then toArray to get the data back

// 5. Find out the number of total words in the book.
// a. You need to split each line into words
// b. For each line, get back the length of the array of words
// c. Call sum to add them.

// 6. Get the ten most popular words in the book
// You will need to use either groupBy, countBy or sumByKey.
// Use sortByDescending to get the "top ten" words