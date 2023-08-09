
using IntentoGoogleAPI.Models;
using IntentoGoogleAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}";
var target = Environment.GetEnvironmentVariable("TARGET") ?? "World";


//-----------------------------------------------------Inyeccion de servicios
builder.Services.AddScoped<LoginService>();
builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson();
Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            webBuilder.UseUrls($"http://*:{port}/");
        });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string []{}
        }
    });
});

//-------------------------------------------------------------Context
builder.Services.AddSqlServer<ContabilidadContext>(builder.Configuration.GetConnectionString("DefaultConnection"));

//---------------------------------------------------------service layer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,

    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("UserType", "1"));
    options.AddPolicy("Propietario", policy => policy.RequireClaim("UserType", "2"));
    options.AddPolicy("Empleado", policy => policy.RequireClaim("UserType", "3"));
    options.AddPolicy("AdminOrPropietario", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim("UserType", "1") || // Admin
            context.User.HasClaim("UserType", "2")    // Propietario
        );
    });
    options.AddPolicy("PropietarioOrEmpleado", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim("UserType", "2") || // Propietario
            context.User.HasClaim("UserType", "3")    // Empleado
        );
    });
});
//------------------------------------------------------CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors();

app.Run();
