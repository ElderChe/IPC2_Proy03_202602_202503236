var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllers();

// AGREGAR ESTE BLOQUE: Le damos permiso para que reciba datos de cualquier puerto (CORS)
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

// Encender el permiso que creamos arriba (tiene que ir ANTES de UseAuthorization)
app.UseCors("DejarPasarTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();