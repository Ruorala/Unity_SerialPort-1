using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

[RequireComponent(typeof(DollHandler))]
public class COMTest : MonoBehaviour
{
    [SerializeField]
    Dropdown comSelect;
    [SerializeField]
    InputField baudRate;
    [SerializeField]
    Dropdown parity;
    [SerializeField]
    InputField datebit;
    [SerializeField]
    Dropdown stopBit;
    [SerializeField]
    InputField readtimeout;
    [SerializeField]
    InputField writetimeout;

    [SerializeField]
    Button connection;
    [SerializeField]
    Button disconnection;
    [SerializeField]
    Button Command_D;
    [SerializeField]
    Button Command_DC;
    [SerializeField]
    Button Command_DS;
    [SerializeField]
    Button Command_Z;
    [SerializeField]
    Button Command_RST;

    [SerializeField]
    Text consoleText;

    // 文末コード
    byte endCode = 0x0d;

    DollHandler dollHandler;

    private void Awake ()
    {
        dollHandler = GetComponent<DollHandler> ();
    }

    // Start is called before the first frame update
    void Start()
    {
        comSelect.value = 0;
        baudRate.text = dollHandler.baudRate.ToString ();
        parity.value = (int)dollHandler._parity;
        datebit.text = dollHandler._databits.ToString ();
        stopBit.value = (int)dollHandler._stopbits;
        readtimeout.text = dollHandler._readTimeout.ToString ();
        writetimeout.text = dollHandler._writeTimeout.ToString ();

        dollHandler.addEventListener (
            (msg) => {
                SetText (msg + "\n");
            },
            (msg) => {
                SetText ("<color>" + msg + "</color>\n");
            }
        );

        comSelect.onValueChanged.AddListener (number => {
            dollHandler.portName = comSelect.options[number].text;
        });
        baudRate.onValueChanged.AddListener (msg => {
            dollHandler.baudRate = int.Parse (msg);
        });
        datebit.onValueChanged.AddListener (msg => {
            dollHandler._databits = int.Parse (msg);
        });
        stopBit.onValueChanged.AddListener (number => {
            dollHandler._stopbits = (StopBits)number;
        });
        parity.onValueChanged.AddListener (number => {
            dollHandler._parity = (Parity)number;
        });
        readtimeout.onValueChanged.AddListener (msg => {
            dollHandler._readTimeout = int.Parse (msg);
        });
        writetimeout.onValueChanged.AddListener (msg => {
            dollHandler._writeTimeout = int.Parse (msg);
        });

        // button
        connection.onClick.AddListener (() => {
            if (dollHandler.Connect () ) {
                connection.interactable = false;
                disconnection.interactable = true;
                Command_D.interactable = true;
                Command_DC.interactable = true;
                Command_DS.interactable = true;
                Command_Z.interactable = true;
                Command_RST.interactable = true;
            }
        });
        disconnection.onClick.AddListener (() => {
            dollHandler.DisConnect ();
            connection.interactable = true;
            disconnection.interactable = false;
            Command_D.interactable = false;
            Command_DC.interactable = false;
            Command_DS.interactable = false;
            Command_Z.interactable = false;
            Command_RST.interactable = false;
        });
        Command_D.onClick.AddListener (() => { dollHandler.WriteCmd ("d"); });
        Command_DC.onClick.AddListener (() => { dollHandler.WriteCmd ("dc"); });
        Command_DS.onClick.AddListener (() => { dollHandler.WriteCmd ("ds"); });
        Command_Z.onClick.AddListener (() => { dollHandler.WriteCmd ("z"); });
        Command_RST.onClick.AddListener (() => { dollHandler.WriteCmd ("rst"); });

        connection.interactable = true;
        disconnection.interactable = false;
        Command_D.interactable = false;
        Command_DC.interactable = false;
        Command_DS.interactable = false;
        Command_Z.interactable = false;
        Command_RST.interactable = false;
    }

    string _msg = "";

    void SetText (string msg)
    {
        // 15000文字以上はエラーが出るので余裕があるあたりで改行１行を削除
        if ( consoleText.text.Length > 10000) {
            int index = consoleText.text.IndexOf('\n');
            consoleText.text = consoleText.text.Substring (index);
        }

        _msg += msg;
    }

    private void Update ()
    {
        if (_msg != "") {
            consoleText.text += _msg;
            _msg = "";
        }
    }
}
