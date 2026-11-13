using System.Text.Json;

namespace mfarinangos6.Views;

public partial class vAgregarEstudiante : ContentPage
{
	private const string Url = "http://192.168.18.29/ws_estudiante/post.php";
	private static readonly HttpClient Cliente = new();

	public vAgregarEstudiante()
	{
		InitializeComponent();
	}

	private async void btnAgregar_Clicked(object? sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtNombre.Text)
			|| string.IsNullOrWhiteSpace(txtApellido.Text)
			|| !int.TryParse(txtEdad.Text, out var edad)
			|| edad <= 0)
		{
			await DisplayAlertAsync("Datos inválidos", "Ingresa nombre, apellido y una edad válida.", "Aceptar");
			return;
		}

		btnAgregar.IsEnabled = false;
		try
		{
			using var contenido = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["nombre"] = txtNombre.Text.Trim(),
				["apellido"] = txtApellido.Text.Trim(),
				["edad"] = edad.ToString()
			});

			using var respuesta = await Cliente.PostAsync(Url, contenido);
			respuesta.EnsureSuccessStatusCode();

			var resultado = await respuesta.Content.ReadAsStringAsync();
			using var documento = JsonDocument.Parse(resultado);
			var raiz = documento.RootElement;
			if (raiz.ValueKind != JsonValueKind.Object
				|| !raiz.TryGetProperty("codigo", out var codigo)
				|| string.IsNullOrWhiteSpace(codigo.ToString()))
			{
				await DisplayAlertAsync("Error", "El servidor no confirmó que se agregó el estudiante.", "Aceptar");
				return;
			}

			await DisplayAlertAsync("Éxito", "Agregado correctamente.", "Aceptar");
			await Navigation.PopToRootAsync();
		}
		catch (HttpRequestException ex)
		{
			await DisplayAlertAsync("Error", $"No se pudo agregar el estudiante. {ex.Message}", "Aceptar");
		}
		catch (TaskCanceledException ex)
		{
			await DisplayAlertAsync("Error", $"La solicitud fue cancelada o excedió el tiempo de espera. {ex.Message}", "Aceptar");
		}
		catch (JsonException ex)
		{
			await DisplayAlertAsync("Error", $"El servidor devolvió una respuesta inválida. {ex.Message}", "Aceptar");
		}
		finally
		{
			btnAgregar.IsEnabled = true;
		}
	}

	private async void btnCancelar_Clicked(object? sender, EventArgs e)
	{
		await Navigation.PopToRootAsync();
	}
}