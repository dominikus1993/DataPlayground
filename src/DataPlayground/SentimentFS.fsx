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

let apiUrl = "http://localhost:5000/api/analysis/"
type SentimentAnalysisResult = JsonProvider<"http://localhost:5000/api/analysis/java">

let sentimentForFsharp = SentimentAnalysisResult.Load(apiUrl + "java")

let getSentimentName(sentiment: int) =
    match sentiment with
    | -2 -> "Bardzo Negatywny"
    | -1 -> "Negatywny"
    | 0 -> "Neutralny"
    | 1 -> "Pozytywny"
    | 2 -> "Bardzo Pozytywny"
    | _ -> "Neutralny"

let sentimentChart = sentimentForFsharp.SentimentByQuantity |> Array.map(fun x -> (getSentimentName(x.Key), x.Value)) |> Chart.Pie |> Chart.WithTitle "Sentiment By Quantity (houseofcards)" |> Chart.WithLegend true


let wordCloud =
    Options(
        title = "Kewords",
        hAxis = Axis(title = "0y"),
        vAxis = Axis(title = "Fertility Rate"),
        bubble = Bubble(textStyle = TextStyle(fontSize = 11))
    )

//let options = Options(showTip = true)
//let locations = sentimentForFsharp.Localizations |> Array.map(fun x -> (x.Latitude.ToString(), x.Longitude.ToString())) |> Chart.Map |> Chart.WithOptions options |> Chart.WithHeight 420