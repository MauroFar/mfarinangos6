using System.Net.Http;
using mfarinangos6.Models;

namespace mfarinangos6.Views;

public partial class vActualizarEliminar : ContentPage
{
	private const string Url = "http://192.168.18.29/ws_estudiante/post.php";
	private static readonly HttpClient Cliente = new();

	public vActualizarEliminar(Estudiante datos)
	{
		InitializeComponent();
		txtCodigo.Text = datos.Codigo.ToString();
		txtNombre.Text = datos.Nombre;
		txtApellido.Text = datos.Apellido;
		txtEdad.Text = datos.Edad.ToString();
	}

	private async void btnActualizar_Clicked(object? sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtCodigo.Text)
			|| string.IsNullOrWhiteSpace(txtNombre.Text)
			|| string.IsNullOrWhiteSpace(txtApellido.Text)
			|| !int.TryParse(txtEdad.Text, out var edad)
			|| edad <= 0)
		{
			await DisplayAlertAsync("Datos inválidos", "Verifica el código, nombre, apellido y edad.", "Aceptar");
			return;
		}

		var url = $"{Url}?codigo={Uri.EscapeDataString(txtCodigo.Text)}"
			+ $"&nombre={Uri.EscapeDataString(txtNombre.Text.Trim())}"
			+ $"&apellido={Uri.EscapeDataString(txtApellido.Text.Trim())}"
			+ $"&edad={edad}";

		await EnviarSolicitud(new HttpRequestMessage(HttpMethod.Put, url), "No se pudo actualizar el estudiante.");
	}

	private async void btnEliminar_Clicked(object? sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtCodigo.Text))
		{
			await DisplayAlertAsync("Error", "No se encontró el código del estudiante.", "Aceptar");
			return;
		}

		var confirmar = await DisplayAlertAsync(
			"Confirmar eliminación",
			"¿Deseas eliminar este estudiante?",
			"Eliminar",
			"Cancelar");

		if (!confirmar)
		{
			return;
		}

		var url = $"{Url}?codigo={Uri.EscapeDataString(txtCodigo.Text)}";
		await EnviarSolicitud(new HttpRequestMessage(HttpMethod.Delete, url), "No se pudo eliminar el estudiante.");
	}

	private async Task EnviarSolicitud(HttpRequestMessage solicitud, string mensajeError)
	{
		using (solicitud)
		{
			try
			{
				using var respuesta = await Cliente.SendAsync(solicitud);
				respuesta.EnsureSuccessStatusCode();
				await DisplayAlertAsync("Éxito", "La operación se realizó correctamente.", "Aceptar");
				await Navigation.PopToRootAsync();
			}
			catch (HttpRequestException ex)
			{
				await DisplayAlertAsync("Error", $"{mensajeError} {ex.Message}", "Aceptar");
			}
			catch (TaskCanceledException ex)
			{
				await DisplayAlertAsync("Error", $"La solicitud fue cancelada o excedió el tiempo de espera. {ex.Message}", "Aceptar");
			}
		}
	}
}