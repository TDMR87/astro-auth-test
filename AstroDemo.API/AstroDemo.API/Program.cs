var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
}); 

var app = builder.Build();

// Just a mock store for fake access tokens
List<string> accessTokens = [];

app.MapGet("/login", (HttpContext context) =>
{
    // This demonstrates an external identity provider's endpoint that 
    // redirects the user to a specific URL after successfull login.
    // We redirect to our Astro frontend page.
    return Results.Redirect("http://localhost:4321/login-callback?code=12345");
});

app.MapGet("/signin", (HttpContext context) =>
{
    // This endpoint is called after successfully logging in with the 
    // identity provider. In real life, exchange the authorization code
    // to an access token / id token in the external identity provider
    // to get user's name, email etc.
    var code = context.Request.Query["code"];

    // ...then create an access token.
    // In real life, we'd generate a JWT.
    var accessToken = Guid.NewGuid().ToString();
    accessTokens.Add(accessToken);
    return Results.Ok(accessToken);
});

// This is a protected endpoint, you need a valid accessToken in order to get data
app.MapGet("/data", (HttpContext context) =>
{
    var accessToken = context.Request.Headers.Authorization
        .ToString()
        .Replace("Bearer", string.Empty)
        .Trim();

    if (string.IsNullOrWhiteSpace(accessToken) || !accessTokens.Contains(accessToken!))
    {
        return Results.Unauthorized();
    }

    return Results.Ok("Protected data from API: " +
        "Ea voluptate eos itaque numquam repellendus a. " +
        "Vel praesentium exercitationem quas tenetur consectetur ab. " +
        "Sed enim aspernatur quaerat. Blanditiis aut ipsum sint aspernatur " +
        "saepe nostrum ut. Minima est fugiat velit non et a iste aut. " +
        "Fugiat ut corporis eaque velit earum omnis eius reiciendis.");
});

app.UseCors();
app.Run();