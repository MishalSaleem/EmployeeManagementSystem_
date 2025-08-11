using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register your ADO.NET services as scoped
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<LocationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // Enforce secure HTTP (HTTPS)
}

app.UseHttpsRedirection();      // Redirect HTTP to HTTPS
app.UseStaticFiles();           // Serve static files (CSS, JS)
app.UseRouting();               // Use routing for endpoints

app.MapBlazorHub();             // Enable SignalR for Blazor
app.MapFallbackToPage("/_Host"); // Fallback to Blazor

app.Run(); // Start the app
