open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    let app = builder.Build()

    app.MapGet("/", fun (httpContext: HttpContext) ->
        Results.Ok("Hello World!")) |> ignore

    app.Run()

    0 // Exit code

