using System.Collections.Generic;

namespace VoicemeeterSliderControl
{
    public class Sliders
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public IList<SliderSet> SliderSets { get; set; }
    }
}