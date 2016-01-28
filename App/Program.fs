// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System.Configuration
open FSharp.Data


let [<Literal>] CnxString = "server=.\SqlExpress;database=AppData;Integrated Security=true"

let [<Literal>] InsertBookQuery = """
INSERT Book
(Title)
VALUES
(@title)

SELECT CAST(@@IDENTITY as int)
"""

type InsertBookCmd = SqlCommandProvider<InsertBookQuery, CnxString, SingleRow = true>

let insert title =
    use cmd = new InsertBookCmd()
    cmd.Execute(title = title) |> Option.get |> Option.get


let [<Literal>] GetBookQuery = """
SELECT * From Book
WHERE Id = @id
"""

type GetBookCmd = SqlCommandProvider<GetBookQuery, CnxString, SingleRow = true>

let getBook id =
    use cmd = new GetBookCmd()
    cmd.Execute(id = id)

[<EntryPoint>]
let main argv = 
    let cnx = ConfigurationManager.ConnectionStrings.["AppData"].ConnectionString
    insert "Antifragile" |> printfn "%A"
    insert "I'm a strange loop" |> printfn "%A"

    getBook 1 |> Option.iter (printfn "%A")

    printfn "%A" argv
    0 // return an integer exit code




















//type InsertBookCmd = SqlCommandProvider<InsertBookQuery, CnxString, SingleRow = true, ConfigFile = Config>


















//let [<Literal>] CnxString = "name=AppData"
//
//#if !LocalDb
//let [<Literal>]Config = "App.config"
//#else
//let [<Literal>]Config = "App.localdb.config"
//#endif
