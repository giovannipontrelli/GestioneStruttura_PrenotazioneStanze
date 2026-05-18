using GestionePrenotazioniStruttura.Data;
using GestionePrenotazioniStruttura.Services;
using GestionePrenotazioniStruttura.Services.Interfaces;
using GestionePrenotazioniStruttura.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Rimuove il mapping automatico dei claim standard
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// CONTROLLERS
builder.Services.AddControllers();

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI SERVICES
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStructureService, StructureService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthorizationHandler, StrutturaScopeHandler>();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================= JWT =================
var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
           ValidateIssuer = false,
           ValidateAudience = false,
           ValidateLifetime = true,
           ClockSkew = TimeSpan.Zero,
           NameClaimType = ClaimTypes.NameIdentifier,
           RoleClaimType = ClaimTypes.Role
       };

       options.Events = new JwtBearerEvents
       {
           OnMessageReceived = context =>
           {
               var authHeader = context.Request.Headers["Authorization"].ToString();
               Console.WriteLine($"RAW AUTH HEADER: [{authHeader}]");

               if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
               {
                   var token = authHeader.Substring("Bearer ".Length).Trim().Trim('"');
                   Console.WriteLine($"TOKEN ASSEGNATO A CONTEXT: [{token}]");
                   context.Token = token;
               }

               return Task.CompletedTask;
           },

           OnAuthenticationFailed = context =>
           {
               Console.WriteLine("JWT FAILED: " + context.Exception);
               return Task.CompletedTask;
           },

           OnTokenValidated = context =>
           {
               Console.WriteLine("TOKEN VALIDATED!");
               return Task.CompletedTask;
           }
       };

   });

/*
 possibili implementazioni ruoli:
Trainer
Revisor(Funzioni "admin" legate alla struttura)
 */

// ================= AUTHORIZATION =================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("RevisorOnly", policy => policy.RequireRole("Revisor"));
    options.AddPolicy("StrutturaScope", policy =>
        policy.Requirements.Add(new StrutturaScopeRequirement()));
});

var app = builder.Build();

// SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();

// MIDDLEWARE AUTH
app.UseAuthentication();
app.UseAuthorization();

// MAP CONTROLLERS
app.MapControllers();

app.Run();
