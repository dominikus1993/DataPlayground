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
            
let text = "awesome great this text is so exciting! this is disgusting sentence number two.";

let sentimentAnalyzer = makeSentimentAnalyzer jarRoot

sentimentAnalyzer text

