using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json")
    .AddEnvironmentVariables();

builder.Services.AddAuthentication(option =>
                    {
                        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).AddJwtBearer(options =>
                    {
                        var secretkey =
                            Encoding.UTF8.GetBytes("mdlkvnkjnmkFEFmfk vi infnojFEFn#$#$   kmffs',rkEFEFisrsd_jn3*^");
                        var encryptKey = Encoding.UTF8.GetBytes("17COaqEn&rLptMey");

                        var validationParameters = new TokenValidationParameters
                        {
                            ClockSkew = TimeSpan.Zero,
                            RequireSignedTokens = true,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                            RequireExpirationTime = true,
                            ValidateLifetime = true,

                            ValidateAudience = true,
                            ValidAudience = "Audience",

                            ValidateIssuer = true,
                            ValidIssuer = "Issuer",

                            TokenDecryptionKey = new SymmetricSecurityKey(encryptKey)
                        };

                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = validationParameters;
                    });
builder.Services.AddOcelot().AddConsul();
var app = builder.Build();
app.UseOcelot().Wait();
app.Run();