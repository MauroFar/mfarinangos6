using System.Collections.ObjectModel;
using System.Text.Json;
using mfarinangos6.Models;

namespace mfarinangos6.Views;

public partial class vEstudiante : ContentPage
{
    private const string Url = "http://192.168.18.29/ws_estudiante/post.php";
    private static readonly HttpClient Cliente = new();

    public ObservableCollection<Estudiante> Estudiantes { get; } = new();

    public vEstudiante()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await MostrarEstudiante();
    }

    private async void Actualizar_Clicked(object? sender, EventArgs e)
    {
        await MostrarEstudiante();
    }

    private async void btnAgregarEstudiante_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new vAgregarEstudiante());
    }

    private async void listaEstudiante_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not Estudiante ObjetoEstudiante)
        {
            return;
        }

        await Navigation.PushAsync(new vActualizarEliminar(ObjetoEstudiante));
    }

    private async Task MostrarEstudiante()
    {
        try
        {
            var respuesta = await Cliente.GetStringAsync(Url);
            var estudiantes = LeerEstudiantes(respuesta);

            Estudiantes.Clear();
            foreach (var estudiante in estudiantes)
            {
                Estudiantes.Add(estudiante);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"No se pudieron cargar los estudiantes.\n{ex.Message}", "Aceptar");
        }
    }

    private static List<Estudiante> LeerEstudiantes(string json)
    {
        using var documento = JsonDocument.Parse(json);
        var raiz = documento.RootElement;

        if (raiz.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<Estudiante>>(raiz.GetRawText()) ?? [];
        }

        if (raiz.ValueKind == JsonValueKind.Object)
        {
            foreach (var propiedad in new[] { "data", "estudiantes", "datos" })
            {
                if (raiz.TryGetProperty(propiedad, out var lista) && lista.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<Estudiante>>(lista.GetRawText()) ?? [];
                }
            }
        }

        return [];
    }
}
