using AspNetBlog.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });//inibe a validação automatica do aspnet do modelo. ModelState.IsValid example.
builder.Services.AddDbContext<BlogDataContext>();
var app = builder.Build();
app.MapControllers();
app.Run();
