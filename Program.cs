using AspNetBlog;
using AspNetBlog.Data;
using AspNetBlog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build();
//app.Configuration.GetSection(); //Faz o parse do settings pra uma classe... 
//LoadConfiguration(app);

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.MapControllers();
app.UseStaticFiles();


app.Run();


void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}

void LoadConfiguration(WebApplication app)
{
    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("JwtKey");

    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache(); // usando cache de memoria;
    builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<GzipCompressionProvider>();
        options.EnableForHttps = true;
        //options.Providers.Add<CustomCompressionProvider>();
    });
    builder
        .Services
        .Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
    builder
        .Services
        .AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
            //inibe a validação automatica do aspnet do modelo. ModelState.IsValid example.
        }).AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault; //se estiver nulo, n renderiza na tela
        });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<BlogDataContext>();
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<EmailService>();
    //builder.Services.AddTransient(); //sempre criará um novo.
    //builder.Services.AddScoped(); // é por transação/Requisição
    //builder.Services.AddSingleton(); // 1por app, mantem na memoria.
}