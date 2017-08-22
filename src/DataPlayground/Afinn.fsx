#load "./AllLibs.fsx"
open Chiron
open System.IO
open SentimentFS.TextUtilities.Text
open SentimentFS.TextUtilities.Tokenizer
open SentimentFS.TextUtilities.Filter
open SentimentFS.Stemmer.Stemmer
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle
open XPlot

let OpenFile(fileName : string) = 
    use reader = new StreamReader(fileName)
    reader.ReadToEnd()

let private stopWords = """a about above after again against all am an and any are aren't as at be
  because been before being below between both but by can't cannot could
  couldn't did didn't do does doesn't doing don't down during each few for from
  further had hadn't has hasn't have haven't having he he'd he'll he's her here
  here's hers herself him himself his how how's i i'd i'll i'm i've if in into
  is isn't it it's its itself let's me more most mustn't my myself no nor not of
  off on once only or other ought our ours ourselves out over own same shan't
  she she'd she'll she's should shouldn't so some such than that that's the
  their theirs them themselves then there there's these they they'd they'll
  they're they've this those through to too under until up very was wasn't we
  we'd we'll we're we've were weren't what what's when when's where where's
  which while who who's whom why why's with won't would wouldn't you you'd
  you'll you're you've your yours yourself yourselves""" |> wordsSequence |> Seq.toList

let IsNegate = function
    | "not" | "don't" | "dont" | "no" | "nope"  -> true
    |_ -> false

let map:Map<string, int> = Json.parse (OpenFile("afinn.json")) |> Json.deserialize

let classify word =
    let (score, _) = word 
                        |> tokenize 
                        |> filterOut(stopWords) 
                        |> List.map(fun x -> x |> stem) 
                        |> List.fold (fun (score, lastWord) word -> 
                                        match map.TryFind(word) with
                                        | Some s -> 
                                            let wordScore = if IsNegate(lastWord) then -s else s
                                            (score + wordScore, word)
                                        | None -> (score, word)
                                     ) (0, "")
    score

let sentiments = Directory.GetFiles("""D:\reviews\txt_sentoken\neg""") 
                                    |> Array.map(File.ReadAllText >> classify)
                                    |> Array.groupBy(fun x -> if x > 0 then "Pozytywny" else "Negatywny")
                                    |> Array.map(fun (x, xs) -> (x, xs |> Array.length))
                                    |> Chart.Pie
    