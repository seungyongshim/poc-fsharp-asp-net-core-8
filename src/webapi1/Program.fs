open FSharp.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open System.Net.Http
open System
open System.Net.Http.Json


type Dto = {
    Name: string
    PhoneNumber : string
}

[<EntryPoint>]
let main _ =
    let app = webApp {
        services (fun services _ ->
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
            let! response = httpClient.PostAsJsonAsync("/api/users", dto)
            let! jsonResopnse = response.Content.ReadFromJsonAsync<Dto>()
            return Results.Json(jsonResopnse, contentType= $"{response.Content.Headers.ContentType}")
        })
    }

    app.Run()
    0 // Exit code


