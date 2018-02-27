using ArduinoDriver;
using ArduinoDriver.SerialProtocol;
using ArduinoUploader.Hardware;
using System;

namespace Hashlamakaton
{
    public class Sensors : IDisposable, ISensor
    {
        private const byte k_SoundSensorPin = 0;
        private const byte k_LightSensorPin = 1;
        private const byte k_TiltSensorPin = 2;
        private const byte k_UltraSonicTrig = 3;
        private const byte k_UltraSonicEcho = 4;

        public bool IsAvilable { get; private set; }

        private const int k_StartPort = 3;
        private static int s_PortNum = k_StartPort;
        private readonly int r_PortNum;

        private ArduinoDriver.ArduinoDriver m_Driver;
        public Sensors(bool i_InfraRedSender = false)
        {
            r_PortNum = s_PortNum++;
            try
            {
                m_Driver = new ArduinoDriver.ArduinoDriver(ArduinoModel.UnoR3, "COM" + r_PortNum, true);
                m_Driver.Send(new PinModeRequest(k_SoundSensorPin, PinMode.Input));
                m_Driver.Send(new PinModeRequest(k_LightSensorPin, PinMode.Input));
                m_Driver.Send(new PinModeRequest(k_TiltSensorPin, PinMode.Input));

                IsAvilable = true;
            }
            catch
            {
                IsAvilable = false;
            }
        }

        public int GetSound()
        {            
            AnalogReadResponse soundSensorResponse = m_Driver.Send(new AnalogReadRequest(k_SoundSensorPin));
            return soundSensorResponse.PinValue;
        }

        public int GetLight()
        {
            AnalogReadResponse soundSensorResponse = m_Driver.Send(new AnalogReadRequest(k_LightSensorPin));
            return soundSensorResponse.PinValue;
        }

        public DigitalValue GetTilt()
        {
            DigitalReadResponse tiltSensorResponse = m_Driver.Send(new DigitalReadRequest(k_TiltSensorPin));
            return tiltSensorResponse.PinValue;
        }

        public object GetCurrent()
        {
            return new
            {
                Type = "Arduino",
                Id = r_PortNum - k_StartPort,
                Sound = GetSound(),
                Light = GetLight(),
                Tilt = GetTilt(),
            };
        }

        public void Dispose()
        {
            ((IDisposable)m_Driver).Dispose();
        }

    }
}
