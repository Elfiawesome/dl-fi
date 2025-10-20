using System.Net;
using System.Text;

namespace DLFI.Webapp;

public class ServerApp
{
	private HttpListener _httpListener = new();
	private StaticHandler staticHandler = new();

	public ServerApp()
	{
	}

	public async Task Run()
	{
		_httpListener.Prefixes.Add("http://localhost:2020/");
		_httpListener.Start();
		while (true)
		{
			var context = await _httpListener.GetContextAsync();
			var req = context.Request;
			var resp = context.Response;
			var url = req?.Url?.AbsolutePath;
			if (url == null || req == null) { continue; }

			if (url.StartsWith("/api"))
			{
			}
			else
			{
				await staticHandler.Handle(req, resp, url);
			}
		}
	}
}

public abstract class RouteHandler
{
	public abstract Task Handle(HttpListenerRequest req, HttpListenerResponse resp, string relPath);

	public static async Task SendData(HttpListenerResponse resp, Stream stream, string contentType = "text/html")
	{
		await stream.CopyToAsync(resp.OutputStream);
		resp.ContentType = contentType;
		resp.Close();
	}
}

public class StaticHandler : RouteHandler
{
	private string _path = @"C:\Users\elfia\OneDrive\Desktop\DL-FI Project\Site\Sketch\v1\";
	public override async Task Handle(HttpListenerRequest req, HttpListenerResponse resp, string relPath)
	{
		if (relPath == "/") { relPath = "index.html"; }
		var filePath = Path.Join(_path, relPath);
		Console.WriteLine(filePath);
		if (File.Exists(filePath))
		{
			var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			await SendData(resp, stream);
			return;
		}

		using MemoryStream ms = new(Encoding.UTF8.GetBytes("Not Found..."));
		await SendData(resp, ms);
	}
}