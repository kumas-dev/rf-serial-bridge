using System.IO.Ports;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using RFSerialBridge.Responses;

namespace RFSerialBridge;

public class Worker : BackgroundService
{
    private readonly SerialPort _serialPort;
    private readonly ILogger<Worker> _logger;
    private readonly Config _config;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public Worker(ILogger<Worker> logger, IOptions<Config> config, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _config = config.Value;
        _httpClientFactory = httpClientFactory;

        _serialPort = new SerialPort(_config.ComPort, _config.BaudRate, Parity.None, 8, StopBits.One)
        {
            Handshake = Handshake.None,
            NewLine = "\n",
            ReadTimeout = 1000,
            WriteTimeout = 1000,
        };

        _serialPort.DataReceived += SerialPort_DataReceived;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
                _logger.LogInformation("SerialPort opened on {PortName}", _serialPort.PortName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SerialPort open failed");
            throw;
        }
        
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                _logger.LogInformation("SerialPort closed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to close SerialPort");
        }

        return base.StopAsync(cancellationToken);
    }

    private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string line = _serialPort.ReadLine(); // NewLine 기준으로 읽음
            _logger.LogInformation($"[RF 수신] {line}");

            
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_config.Host}/health");
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK: // 200
                    var data = await response.Content.ReadFromJsonAsync<HealthResponse>();
                    if (data != null)
                    {
                        _logger.LogInformation("서버 상태: {Status}", data.Status);
                    }
                    
                    _logger.LogInformation(data.Status);

                    break;

                case HttpStatusCode.BadRequest: // 400
                    _logger.LogWarning("요청이 잘못되었습니다 (400)");
                    break;

                case HttpStatusCode.Unauthorized: // 401
                    _logger.LogWarning("인증이 필요합니다 (401)");
                    break;

                case HttpStatusCode.InternalServerError: // 500
                    _logger.LogError("서버 오류 발생 (500)");
                    break;

                default:
                    _logger.LogWarning("예상치 못한 응답: {StatusCode}", (int)response.StatusCode);
                    break;
            }
        }
        catch (TimeoutException)
        {
            _logger.LogError("Read timeout");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Serial read error: {ex.Message}");
        }
    }
}