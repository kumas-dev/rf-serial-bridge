namespace RFSerialBridge;

public class Config 
{
    public string ComPort { get; set; } = "COM3";
    public int BaudRate { get; set; } = 9600;
    
    public string Host { get; set; } = "http://localhost:5500";
}