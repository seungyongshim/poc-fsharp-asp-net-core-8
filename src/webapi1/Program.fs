open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open System.Net.Http
open Microsoft.AspNetCore.Http

#nowarn "0020"

[<CLIMutable>]
type Dto = {
    Name: string
}

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddHttpClient("reqres", fun (http: HttpClient) ->
        http.BaseAddress <- Uri("https://reqres.in/api/")
    ).AddAsKeyed() 

    let app = builder.Build()

    app.MapPost("/", Func<HttpContext,HttpClient,Dto,_>(fun context -> fun client -> fun dto -> task {
          return "Hello World!"
        })) 

    app.Run()

    0 // Exit code

