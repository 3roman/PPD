using Mvvm;

namespace PipePressureDrop.Model
{
    internal class Report : BindableBase
    {
        public string Item { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
