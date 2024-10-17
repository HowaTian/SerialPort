using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Security.Policy;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SerialPortController : MonoBehaviour
{
    private SerialPort serialPort;
    public string portName = "COM3";
    public int baudRate = 115200;
    private string dataStr = string.Empty;

    public Thread readThread;
    public Thread writeThread;

    public Button openSerialPortBtn;
    public Button closeSerialPortBtn;
    public Text dataText;
    public Slider slider;
    private int sliderValue;
    // Start is called before the first frame update
    void Start()
    {
        openSerialPortBtn.onClick.AddListener(OpenSerialPort);
        closeSerialPortBtn.onClick.AddListener(CloseSerialPort);
    }

    private void CloseSerialPort()
    {
        if(serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            readThread.Abort();
            writeThread.Abort();
        }
    }

    private void OpenSerialPort()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();

        readThread = new Thread(ReadData);
        readThread.Start();

        writeThread = new Thread(WriteData);
        writeThread.Start();
        
    }

    private void WriteData(object obj)
    {
        byte[] cmd = new byte[2];
        cmd[0] = 0xA3;
        cmd[1] = 0xFE;

        while(serialPort.IsOpen)
        {
            serialPort.Write(cmd, 0, 2);
            Thread.Sleep(30);
        }
    }

    private void ReadData(object obj)
    {
        while (serialPort.IsOpen)
        {
            int count = serialPort.BytesToRead;
            if (count < 24) continue;
            if(count > 0)
            {
                byte[] buffer = new byte[count];
                serialPort.Read(buffer, 0, count);
                sliderValue = buffer[6];
                dataStr = BitConverter.ToString(buffer);
                Thread.Sleep(30);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        dataText.text = dataStr;
        slider.value = sliderValue;
    }
}
