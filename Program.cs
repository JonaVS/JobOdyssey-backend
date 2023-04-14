using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JobOdysseyApi.Configurations;
using JobOdysseyApi.Data;
using JobOdysseyApi.Filters;
using JobOdysseyApi.Models;
using JobOdysseyApi.Services;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddSwaggerGen();
builder.Services.AddSqlite<AppDbContext>(DotNetEnv.Env.GetString("DB_CONNECTION"));
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddJwtAuthentication();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedEmail = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<AuthTokensService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<RefreshTokenValidationFilterAttribute>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
