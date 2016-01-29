// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System.Configuration
open FSharp.Data



#if !LocalDb
let [<Literal>]Config = "App.config"
#else
let [<Literal>]Config = "App.localdb.config"
#endif

//let [<Literal>] CnxString = "server=.\SqlExpress;database=AppData;Integrated Security=true"
let [<Literal>] CnxString = "name=AppData"

let [<Literal>] InsertBookQuery = """
INSERT Books
(Title, Author)
VALUES
(@title, @author)

SELECT CAST(@@IDENTITY as int)
"""

type InsertBookCmd = SqlCommandProvider<InsertBookQuery, CnxString , SingleRow = true, ConfigFile = Config>

let insert title author =
    use cmd = new InsertBookCmd()
    cmd.Execute(title = title, author = author) |> Option.get |> Option.get


let [<Literal>] GetBookQuery = """
SELECT * From Books
WHERE Id = @id
"""

type GetBookCmd = SqlCommandProvider<GetBookQuery, CnxString, SingleRow = true, ConfigFile = Config>

let getBook id =
    use cmd = new GetBookCmd()
    cmd.Execute(id = id)

type Data = JsonProvider< "../Api/Data.json", SampleIsList = true >


[<EntryPoint>]
let main argv = 
    let cnx = ConfigurationManager.ConnectionStrings.["AppData"].ConnectionString
    insert "Antifragile" |> printfn "%A"
    insert "I'm a strange loop" |> printfn "%A"

    getBook 1 |> Option.iter (fun b -> 
        let author = defaultArg b.Author "Anonymous"
        printfn "%s by %s" b.Title author)

    printfn "%A" argv

    let b = Data.Parse """{
    "text": "This is some json",
    "value": 314 }"""

    printfn "%s (%d)" b.Text (defaultArg b.Value 0)

    0 // return an integer exit code




















//type InsertBookCmd = SqlCommandProvider<InsertBookQuery, CnxString, SingleRow = true, ConfigFile = Config>

















