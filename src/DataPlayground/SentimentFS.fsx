// Before running any code, invoke Paket to get the dependencies.
//
// You can either build the project (Ctrl + Alt + B in VS) or run
// '.paket/paket.bootstrap.exe' and then '.paket/paket.exe install'
// (if you are on a Mac or Linux, run the 'exe' files using 'mono')
//
// Once you have packages, use Alt+Enter (in VS) or Ctrl+Enter to
// run the following in F# Interactive. You can ignore the project
// (running it doesn't do anything, it just contains this script)
#load "..\..\packages/FsLab/FsLab.fsx"

open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle
open XPlot
let apiUrl = "http://localhost:5000/api/tweets/"
type SentimentAnalysisResult = JsonProvider<"http://localhost:5000/api/tweets/fsharp">
let key = "houseofcards"

let sentimentByKey(key: string) = SentimentAnalysisResult.Load(apiUrl + key)
let sentimentForFsharp = sentimentByKey(key)



let getSentimentName(sentiment: int) =
    match sentiment with
    | -2 -> "Bardzo Negatywny"
    | -1 -> "Negatywny"
    | 0 -> "Neutralny"
    | 1 -> "Pozytywny"
    | 2 -> "Bardzo Pozytywny"
    | _ -> "Neutralny"

let sentimentChart = sentimentForFsharp.TweetList |> Array.groupBy(fun x -> x.Sentiment) |> Array.map(fun (sent, tweets) -> (getSentimentName(sent), tweets |> Array.length)) |> Chart.Pie |> Chart.WithTitle (sprintf "Sentiment By Quantity (%s)" key) |> Chart.WithLegend true


// let dateByQuantityOptions =
//     Options(
//         title = "Date By Quantity",
//         height = 350
//     )

// sentimentForFsharp.DateByQuantity |> Array.map(fun x -> (x.Key, x.Value)) |> Chart.Calendar |> Chart.WithOptions dateByQuantityOptions