#load "..\..\packages/FsLab/FsLab.fsx"
#r "..\..\packages/IKVM/lib/IKVM.OpenJDK.Core.dll"
#r "..\..\packages/IKVM/lib/IKVM.OpenJDK.Util.dll"
#r "..\..\packages/Stanford.NLP.CoreNLP/lib/stanford-corenlp-3.7.0.dll"

open System
open System.IO
open java.util
open java.io
open edu.stanford.nlp.pipeline