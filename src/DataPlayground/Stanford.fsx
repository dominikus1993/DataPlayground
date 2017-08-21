#load "./AllLibs.fsx"

open System
open System.IO
open java.util
open java.io
open edu.stanford.nlp.pipeline
open edu.stanford.nlp.ling
open edu.stanford.nlp.neural.rnn
open edu.stanford.nlp.sentiment
open edu.stanford.nlp.trees
open edu.stanford.nlp.util
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle
open XPlot

let jarRoot = __SOURCE_DIRECTORY__ + @"..\..\..\data\models"

printfn "root: %A" jarRoot

let classForType<'t> =
    java.lang.Class.op_Implicit typeof<'t>

type SentimentPrediction = 
    | VeryNegative
    | Negative
    | Neutral
    | Positive
    | VeryPositive

let classToSentiment = function
    | 0 -> VeryNegative
    | 1 -> Negative
    | 2 -> Neutral
    | 3 -> Positive
    | 4 -> VeryPositive
    | _ -> failwith "unknown class"


let makeSentimentAnalyzer modelsDir =
    let props = Properties()
    props.setProperty("annotators", "tokenize, ssplit, pos, parse, sentiment") |> ignore

    let currDir = Environment.CurrentDirectory
    Directory.SetCurrentDirectory modelsDir
    let pipeline = StanfordCoreNLP(props)
    Directory.SetCurrentDirectory currDir
   
    fun text -> 
    (pipeline.``process`` text).get classForType<CoreAnnotations.SentencesAnnotation> :?> ArrayList
            |> Seq.cast<CoreMap>
            |> Seq.map(fun cm -> cm.get classForType<SentimentCoreAnnotations.SentimentAnnotatedTree>)
            |> Seq.cast<Tree>
            |> Seq.map (RNNCoreAnnotations.getPredictedClass >> classToSentiment)
            |> Seq.toList


let sentimentAnalyzer = makeSentimentAnalyzer jarRoot

let sentimentToString = function
   | VeryNegative -> "VeryNegative"
   | Negative -> "Negative"
   | Neutral -> "Neutral"
   | Positive -> "Positive"
   | VeryPositive -> "VeryPositive"

let sentiments = Directory.GetFiles("""D:\reviews\txt_sentoken\pos""") 
                                    |> Array.collect(File.ReadAllText >> sentimentAnalyzer >> List.toArray)
                                    |> Array.groupBy(id)
                                    |> Array.map(fun (x, xs) -> (x |> sentimentToString, xs |> Array.length))
                                    |> Chart.Pie
      


