using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

internal class Program
{
    private static readonly ConcurrentDictionary<WebSocket, bool> Clients = new();
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Enable static file serving
        app.UseStaticFiles();
        app.UseRouting();
        app.UseWebSockets(); // Enable WebSocket support

        // Configure default MVC route
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        // WebSocket route for video stream
        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Clients.TryAdd(webSocket, true);
                Console.WriteLine("New client connected.");

                // Set up FFmpeg process
                string ffmpegPath = "C:\\ffmpeg-4.2.1-win64-static\\bin\\ffmpeg.exe";
                Process ffmpeg = null;
                var buffer = new byte[1024 * 16]; // Increase buffer size if necessary

                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        // Start FFmpeg if not running or after a camera switch
                        if (ffmpeg == null || ffmpeg.HasExited)
                        {
                            ffmpeg = new Process
                            {
                                //StartInfo = new ProcessStartInfo
                                //{
                                //    FileName = ffmpegPath,
                                //    Arguments = "-f webm -i pipe:0 -acodec libopus -f flv rtmp://192.168.2.167/live/stream",
                                //    RedirectStandardInput = true,
                                //    UseShellExecute = false,
                                //    CreateNoWindow = true
                                //}

                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = ffmpegPath,
                                    Arguments = "-f webm -i pipe:0 -c:v libx264 -preset veryfast -tune zerolatency -g 30 -r 30 -c:a aac -b:a 128k -bufsize 64k -f flv rtmp://192.168.2.96/live/stream",
                                    RedirectStandardInput = true,
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                }
                            };
                            ffmpeg.Start();
                            Console.WriteLine("FFmpeg process started.");
                        }

                        // Continuously receive and send the WebSocket video stream to FFmpeg
                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        int i = 1;

                        // Inner loop for handling incoming frames and FFmpeg streaming
                        while (!result.CloseStatus.HasValue && webSocket.State == WebSocketState.Open)
                        {
                            try
                            {
                                if (result.MessageType == WebSocketMessageType.Binary)
                                {
                                    i++;
                                    Console.WriteLine($"Send frame #{i}");
                                    ffmpeg.StandardInput.BaseStream.Write(buffer, 0, result.Count);
                                    ffmpeg.StandardInput.BaseStream.Flush();
                                }
                                // Receive next frame
                                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Inner loop error: {ex.Message}");
                                break;
                            }
                        }
                    }

                    // Close FFmpeg process when WebSocket is closed
                    if (ffmpeg != null)
                    {
                        ffmpeg.StandardInput.Close();
                        ffmpeg.WaitForExit();
                        ffmpeg.Dispose();
                        Console.WriteLine("FFmpeg process closed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Clients.TryRemove(webSocket, out _);
                }
                finally
                {
                    ffmpeg?.Dispose();
                    Clients.TryRemove(webSocket, out _);
                    Console.WriteLine("Client disconnected.");
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
            }
        });


        // Method to broadcast the video chunk to all connected clients
        async Task BroadcastAsync(ArraySegment<byte> buffer)
        {
            foreach (var client in Clients.Keys.ToList())
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
                }
                else
                {
                    // Remove clients that are not open
                    Clients.TryRemove(client, out _);
                }
            }
        }

        app.Run();
    }

    private static async Task BroadcastAsync(ArraySegment<byte> buffer)
    {
        foreach (var client in Clients.Keys)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }
}