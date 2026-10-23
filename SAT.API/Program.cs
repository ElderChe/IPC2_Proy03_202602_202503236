var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("DejarPasarTodo", politicas =>
    {
        politicas.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();


app.UseCors("DejarPasarTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();