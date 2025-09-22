global using GovElec.Shared;
global using GovElec.App.Services;

global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;

global using System.Net.Http;                 // <- IMPORTANT
global using System.Net.Http.Headers;
global using System.Threading;
global using System.Threading.Tasks;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using System.Text;
global using Microsoft.Extensions.DependencyInjection;