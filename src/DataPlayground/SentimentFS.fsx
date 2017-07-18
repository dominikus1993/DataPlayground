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
#load "./Utils.fsx"

open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle
open XPlot
open Utils
let apiUrl = "http://localhost:5000/api/tweets/"
let googleGeoCodeApiUrl (lat: float) (lng: float) = sprintf "https://maps.googleapis.com/maps/api/geocode/json?latlng=%f,%f&key=%s" lat lng (System.Environment.GetEnvironmentVariable("GoogleApiKey"))
type GoogleGeoData = JsonProvider<"../../google.json">
type SentimentAnalysisResult = JsonProvider<"http://localhost:5000/api/tweets/java">
let key = "java"

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

let s = sentimentForFsharp.TweetList |> Array.groupBy(fun x -> x.Sentiment) |> Array.map(fun (sent, tweets) -> (getSentimentName(sent), tweets |> Array.length)) |> Chart.Pie |> Chart.WithTitle (sprintf "Sentiment By Quantity (%s)" key) |> Chart.WithLegend true

let dateByQuantityOptions =
    Options(
        title = "Date By Quantity",
        height = 350
    )

let dataByQuantityChart = sentimentForFsharp.TweetList |> Array.groupBy(fun x -> x.CreatedAt.Date) |> Array.map(fun (date, tweets) -> (date, tweets |> Array.length)) |> Chart.Calendar |> Chart.WithOptions dateByQuantityOptions

let mostPopularKeyWords = sentimentForFsharp.TweetList
                            |> Array.toList
                            |> List.collect(fun x -> x.Text |> Tokenizer.tokenize)
                            |> Filter.filterOut Constants.stopWords
                            |> List.filter(fun word -> word.Length > 3)
                            |> List.groupBy (id)
                            |> List.sortByDescending(fun (word, words) -> words |> List.length)
                            |> List.take 10
                            |> List.map(fun (k ,_) -> k)
 
let sk = sentimentForFsharp.TweetList
            |> Array.toList
            |> List.map(fun x -> (x.Sentiment, x.Text |> Tokenizer.tokenize |> Filter.filterIn mostPopularKeyWords))
            |> List.collect(fun (sentiment, words) -> words |> List.groupBy(id) |> List.map(fun (w, ws) -> (w, getSentimentName(sentiment), ws |> List.length)))
            |> Chart.Sankey
            |> Chart.WithHeight 300
            |> Chart.WithWidth 600



let loadGeoData lat lng = 
    let res = GoogleGeoData.Load(googleGeoCodeApiUrl(lat)(lng))
    res.Results |> Array.collect(fun res -> res.AddressComponents) |> Array.filter(fun ac -> ac.Types |> Array.exists(fun tp -> tp = "country")) |> Array.head


let geoData = sentimentForFsharp.TweetList
                |> Array.filter(fun tweet -> tweet.Longitude <> 0m || tweet.Latitude <> 0m)
                |> Array.map(fun tweet -> ((loadGeoData (tweet.Latitude |> float) (tweet.Longitude |> float)).ShortName.String), 1)
                |> Array.filter(fun (name, _) -> match name with Some _ -> true | None -> false)
                |> Array.map(fun (name, value) -> (name.Value, value))
                |> Array.groupBy(fun (name, value) -> name)
                |> Array.map(fun (name, rest) -> (name, rest |> Array.sumBy(fun (_, value) -> value)))

let geoChart = Chart.Geo(geoData, Labels=["Name"; "Popularity"])

