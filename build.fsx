#I "packages/build/fake/tools"
#r "fakelib.dll"
#r "Fake.SQL"
#r "System.Data"
#r "Microsoft.SqlServer.smo"
#r "Microsoft.SqlServer.Management.sdk.sfc"
#r "Microsoft.SqlServer.ConnectionInfo"
//#r "Microsoft.Web.XmlTransform"
#r "System.Transactions"
#I @"packages\build\Microsoft.SqlServer.Dac\lib"
#r "Microsoft.SqlServer.Dac"
#r "Microsoft.Data.Tools.Schema.Sql"

open System
open Fake
open Fake.SQL

module LocalDb =
    open System.Data.SqlClient
    open Microsoft.SqlServer.Dac

    let localDbName = @"(LocalDB)\MSSQLLocalDB"

    let connectionString path name =
        let fullPath = currentDirectory </> path </> name + ".mdf"
        sprintf "Data Source=%s;AttachDbFilename=%s;Integrated Security=true;Connect Timeout=10" localDbName fullPath

    let localCnx name =
        sprintf @"server=%s;Integrated Security=True;database=%s" localDbName name

    let getServerInfo name = 
        localCnx name
        |> SqlServer.getServerInfo


        

    let createDb path name =
        logfn "Creating db %s in %s" name path
        let fullpath = currentDirectory </> path </> name

        let query = sprintf """
            IF (EXISTS (SELECT * FROM master.dbo.sysdatabases WHERE ( name = '%s')))
                DROP DATABASE %s
            CREATE DATABASE %s
                   ON PRIMARY
                   ( NAME = %s_dat, FILENAME = '%s.mdf')
                   LOG ON
                   ( NAME = %s_log, FILENAME = '%s.ldf')"""
                        name name name name fullpath name fullpath
        
        use cnx = new SqlConnection(localCnx "master")
        use cmd = new SqlCommand(query, cnx)
        cnx.Open()
        cmd.ExecuteNonQuery() |> ignore
       
    let deployDacpac path  name =
        //logfn "Deploying %s.dacpac" name
        logfn "Deploying %s.dacpac" name
        let dacService = DacServices(localCnx "master")
        use pack = DacPackage.Load  (path </> name + ".dacpac")

        dacService.Deploy(pack, name, true)

let bin = "bin"
let appDir = bin </> "app"
let sqlDir = bin </> "sql"
let dbDir = bin </> "db"
let db = "AppData"

Target "Clean" <| fun _ ->
    DeleteDir bin

Target "BuildSql" <| fun _ ->
    !! "**/*.sqlproj"
    |> MSBuildRelease sqlDir  "Rebuild"
    |> Log "[Sql]"

Target "LocalDb" <| fun _ ->
    CreateDir dbDir
    LocalDb.createDb dbDir db
    LocalDb.deployDacpac sqlDir db
    LocalDb.getServerInfo db
    |> SqlServer.Detach 
    |> SqlServer.Disconnect
 
  

Target "Build" <| fun _ ->
    let localConfig = "App/App.localdb.config"
    "App/App.config"
    |> CopyFile localConfig

    localConfig
    |> updateConnectionString "AppData" (LocalDb.connectionString dbDir db)
   
    !! "**/*.fsproj"
    |> MSBuildReleaseExt appDir ["DefineConstants", "LocalDb" ] "Rebuild"
    |> Log "[Build]"

    DeleteFile localConfig

    
Target "All" DoNothing

"Clean"
    ==> "BuildSql"
    ==> "LocalDb"
    ==> "Build"
    ==> "All"


RunTargetOrDefault "All"