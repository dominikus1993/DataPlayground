module Filter =
    let filterOut(filterList: string list)(list: string list) =
        list |> List.filter(fun x -> not (filterList |> List.exists(fun y -> x = y)))
    let filterIn(filterList: string list)(list: string list) =
        list |> List.filter(fun x -> filterList |> List.exists(fun y -> x = y))

    let filterOutSeq(filterSeq: string seq)(list: string seq) =
        filterOut(filterSeq |> Seq.toList)(list |> Seq.toList) |> List.toSeq

module Tokenizer =
    open System
    open System.Text.RegularExpressions

    let replace (pattern: string)(replacement: string)(word: string) =
        Regex.Replace(word, pattern, replacement)

    let split([<ParamArray>]patterns: char array)(text: string) =
        text.Split patterns

    let toLower(word: string) =
        word.ToLower()
    let tokenize =
        split [|' '|]
            >> Array.map(replace ("\W") ("") >> toLower)
            >> Array.filter(String.IsNullOrEmpty >> not)
            >> Array.toList

    let wordsSequence =
        split [|' '; '\n'|]
            >> Array.filter(String.IsNullOrEmpty >> not)
            >> Array.map(replace("\n")(String.Empty) >> replace("\r")(String.Empty)
            >> replace("\t")(String.Empty))
            >> Array.toSeq

module Constants = 
    let stopWords = """a about above after again against all am an and any are aren't as at be
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
      you'll you're you've your yours yourself yourselves""" |> Tokenizer.wordsSequence |> Seq.toList