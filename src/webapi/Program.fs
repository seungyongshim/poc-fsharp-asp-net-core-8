open FSharp.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open System.Net.Http
open System
open System.Net.Http.Json
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Http.Json
open Microsoft.Extensions.Options

type Name = Name of string
type InternationalPhoneNumber = InternationalPhoneNumber of string
type PhoneNumber = InternationalPhoneNumber of InternationalPhoneNumber


[<CLIMutable>]
type Dto = {
    Name: Name
    PhoneNumber : PhoneNumber
}

[<EntryPoint>]
let main _ =
    let app = webApp {
        services (fun services _ ->
            services.ConfigureHttpJsonOptions(fun options ->
                options.SerializerOptions.Converters.Add(JsonFSharpConverter())
            ) |> ignore
            services.AddHttpClient("reqres", fun (http: HttpClient) ->
                http.BaseAddress <- Uri("https://reqres.in")
            ).AddAsKeyed()
        )
        
        get "/" (fun () -> task {
            return Results.Ok("Hello World")
        })

        post "/api/v2/users" (fun (httpContext: HttpContext) ->
                              fun (dto: Dto) -> backgroundTask {
            use httpClient = httpContext.RequestServices.GetRequiredKeyedService<HttpClient>("reqres")
            let options = httpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions

            let! response = httpClient.PostAsJsonAsync("/api/users", dto, options)
            let! stream = response.Content.ReadAsStreamAsync()
            return Results.Stream(stream, contentType= $"{response.Content.Headers.ContentType}")
        })
    }

    app.Run()
    0 // Exit code


