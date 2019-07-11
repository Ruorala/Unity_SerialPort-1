using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO.Ports;
using UnityEngine;

public class DollHandler : SerialConnector
{
    // delegate で　EventDispatcher を作成
    public delegate void Dispatcher (string msg);
    private Dispatcher _dispatcher; // Dispatcher をインスタンス化
    private Dispatcher _error;

    // Dispatcher のコールバック関数
    public void addEventListener (Dispatcher dispatcher, Dispatcher error)
    {
        _dispatcher += dispatcher;
        _error += error;
    }

    void Awake ()
    {
        // シリアルポート設定
        baudRate = 115200;
        _databits = 8;
        _parity = Parity.None;
        _stopbits = StopBits.One;

        _readTimeout = 1000;
        _writeTimeout = 1000;
        
        //_parity = Parity.None;
        //_databits = 8;
        //_stopbits = StopBits.One;
        //_readTimeout = 500;
        //_writeTimeout = 1000;

        // オープンしたら読み取りスレッドを開始
        //if ( open () ) {
        //    //
        //    _isRunning = true;
        //    _thread = new Thread (run);
        //    _thread.Start ();

        //    //
        //    _serialPort.Write ("1");
        //    _serialPort.Write ("2XS\r");
        //}
    }

    public bool Connect()
    {
        if (open()) {
            _isRunning = true;
            _thread = new Thread (run);
            _thread.Start ();

            //WriteCmd ("d");
            return true;
        }
        return false;
    }

    public void DisConnect()
    {
        close ();
    }

    /// <summary>
    /// Read
    /// </summary>
    public override void serialRead ()
    {
        try {
            string data = _serialPort.ReadTo("\r");

            _dispatcher (data);
        }
        catch (System.Exception e) {
            _error (e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    public void WriteCmd(string msg)
    {
        if (_serialPort.IsOpen) {
            _serialPort.Write (msg);
            _dispatcher (msg);
        }
    }
}
