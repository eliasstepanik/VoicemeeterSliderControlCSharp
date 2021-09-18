using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VoicemeeterSliderControl
{
    class Program
    {
        static SerialPort _serialPort;
        static async Task Main(string[] args)
        {
            string fileName = "Config.json";
            string jsonString = File.ReadAllText(fileName);
            Sliders sliders = JsonSerializer.Deserialize<Sliders>(jsonString);
            
            _serialPort = new SerialPort();
            _serialPort.PortName = sliders.PortName;
            _serialPort.BaudRate = sliders.BaudRate;
            _serialPort.Open();

            /*Sliders slider = new Sliders()
            {
                SliderSets = new List<SliderSet>()
                {
                    new SliderSet()
                    {
                        Slider = 0,
                        VoicemeeterValue = "Strip[0].Gain"
                    },
                    new SliderSet()
                    {
                        Slider = 1,
                        VoicemeeterValue = "Strip[1].Gain"
                    }
                }
            };
            string fileNameSave = "Slider.json";
            using FileStream createStream = File.Create(fileNameSave);
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            await JsonSerializer.SerializeAsync(createStream, slider);
            await createStream.DisposeAsync();*/

            using (var _ = await VoiceMeeter.Remote.Initialize(Voicemeeter.RunVoicemeeterParam.VoicemeeterPotato).ConfigureAwait(false))
            {
                int startSkip = 0;
                while (true)
                {
                    string rawInput = _serialPort.ReadLine();
                    
                    if (startSkip++ < 30)
                        continue;
                    if(rawInput.Equals(string.Empty))
                        return;
                    
                    List<float> valuesFloat = ParseValuesFloat(rawInput);

                    foreach (var slidersSlider in sliders.SliderSets)
                    {
                        var parameter = slidersSlider.VoicemeeterValue;
                        float setValue = map(valuesFloat[slidersSlider.Slider], 0f, 100f, -60f, 12f); 
                        VoiceMeeter.Remote.SetParameter(parameter, setValue);
                    }
                    


                }
                
            }
        }

        public static List<float> ParseValuesFloat(string input)
        {
            List<float> floats = new List<float>();

            string[] valueStrings = input.Split("|");
            valueStrings[10] = valueStrings[10].Replace("\r", "");

            foreach (var value in valueStrings)
            {
                if(!value.Contains("|") && !value.Equals(" ") && !value.Equals(String.Empty))
                    floats.Add(map(int.Parse(value), 0, 1024, 0f, 100f));
            }

            return floats;
        }
        
        public static float map (float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}