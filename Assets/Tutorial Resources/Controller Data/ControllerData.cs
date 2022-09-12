using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class ControllerData : MonoBehaviour
{
    private InputDevice _rightController;
    private InputDevice _leftController;
    private InputDevice _HMD;
    // Start is called before the first frame update
    void Start()
    {

    }
    
    void InitializeInputDevices()
    {
        InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref _rightController);
        InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref _leftController);
        InitializeInputDevice(InputDeviceCharacteristics.HeadMounted, ref _HMD);
    }

    void InitializeInputDevice(InputDeviceCharacteristics deviceCharactersitics,ref InputDevice inputDevice)
    {
        List<InputDevice> devices = new List<InputDevice>();
        //Call InputDevices to see if it can find any devices with the characteristics we're looking for
        InputDevices.GetDevicesWithCharacteristics(deviceCharactersitics, devices);
        if (devices.Count > 0)
        {
            inputDevice = devices[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
