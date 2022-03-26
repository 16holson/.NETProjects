using Microsoft.AspNetCore.ResponseCompression;
using Speed.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//--------------------------------------------------------------------------------------------------------------
// Added signalr and text compression for speed project
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options =>
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" })
);

//--------------------------------------------------------------------------------------------------------------

var app = builder.Build();

//--------------------------------------------------------------------------------------------------------------
// Added to go with the text compression
app.UseResponseCompression();
//--------------------------------------------------------------------------------------------------------------


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();

//--------------------------------------------------------------------------------------------------------------
// Register the game hub as a hub endpoint


app.MapHub<GameHub>("/gamehub");
//--------------------------------------------------------------------------------------------------------------

app.MapFallbackToFile("index.html");

app.Run();
