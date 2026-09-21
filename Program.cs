using BubbleSheetReader.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<ImageProcessingService>();
builder.Services.AddScoped<BubbleDetectionService>();
builder.Services.AddScoped<IHandwritingService, HandwritingStubService>();
builder.Services.AddScoped<BubbleSheetProcessor>();
builder.Services.AddScoped<ExcelService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
