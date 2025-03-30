open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

#nowarn "0020"

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    let app = builder.Build()


    app.MapGet("/", Func<_>(fun () -> "Hello World!"))

    app.Run()

    0 // Exit code

