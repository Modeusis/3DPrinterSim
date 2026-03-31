using UnityEngine;

namespace _Project.Scripts.Utilities.Events
{
    public struct OnPlugStateChanged
    {
        public bool IsPlugged { get; set; }
    }

    public struct OnPowerStateChanged
    {
        public bool IsOn { get; set; }
    }

    public struct OnFilamentStateChanged
    {
        public bool IsPlaced { get; set; }
    }
    
    public struct OnDoorStateChanged
    {
        public bool IsOpen { get; set; }
    }
    
    public struct OnFilamentColorChanged
    {
        public Color Color { get; set; }
    }
    
    public struct OnPrintProcessStarted { }
    public struct OnPrintProcessFinished { }
    public struct OnPrinterPanelCleared { }
}