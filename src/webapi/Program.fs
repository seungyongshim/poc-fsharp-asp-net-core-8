open FSharp.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open System.Net.Http
open System
open System.Net.Http.Json
open System.Text.Json.Serialization

[<JsonFSharpConverter>]
type Name = Name of string

[<JsonFSharpConverter>]
type InternationalPhoneNumber = InternationalPhoneNumber of string

type Dto = {
    Name: Name
    PhoneNumber : InternationalPhoneNumber
}

let post_json (url:string) dto (http: HttpClient) = http.PostAsJsonAsync(url, dto)

[<EntryPoint>]
let main _ =
    let app = webApp {
        services (fun services _ ->
            services.AddHttpClient("reqres", fun (http: HttpClient) ->
                http.BaseAddress <- Uri "https://reqres.in"
            ).AddAsKeyed()
        )
        
        get "/" (fun (httpContext: HttpContext) -> task {
            use http = httpContext.RequestServices.GetRequiredKeyedService<HttpClient> "reqres"

            let request = post_json "/api/users" {| Name = "John Doe" |}
            let! response = request http
            let! json_res = response.Content.ReadFromJsonAsync<Dto>()

            return Results.Ok {|
                Name = json_res.Name
            |}
        })

        post "/api/v2/users" (fun (httpContext: HttpContext) (dto: Dto) -> backgroundTask {
            use http = httpContext.RequestServices.GetRequiredKeyedService<HttpClient> "reqres"

            let request = post_json "/api/users" {|
                Name = dto.Name
                PhoneNumber = dto.PhoneNumber
            |}

            let! response = http |> request
            let! stream = response.Content.ReadAsStreamAsync()
            return Results.Stream(stream, contentType= $"{response.Content.Headers.ContentType}")
        })
    }

    app.Run()
    0 // Exit code
