using UnityEngine;
using _Project.Scripts.Setups.Printer;

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
    
    public struct OnPrintProcessStarted
    {
        public ModelSetup Model { get; }
        public SpeedType SpeedType { get; }
        public float SpeedValue { get; }

        public OnPrintProcessStarted(ModelSetup model, SpeedType speedType, float speedValue)
        {
            Model = model;
            SpeedType = speedType;
            SpeedValue = speedValue;
        }
    }

    public struct OnPrintProcessFinished
    {
        public ModelSetup Model { get; }
        public SpeedType SpeedType { get; }
        public float SpeedValue { get; }

        public OnPrintProcessFinished(ModelSetup model, SpeedType speedType, float speedValue)
        {
            Model = model;
            SpeedType = speedType;
            SpeedValue = speedValue;
        }
    }

    public struct OnPrintProgressChanged
    {
        public float ProgressPercent { get; }

        public OnPrintProgressChanged(float progressPercent)
        {
            ProgressPercent = progressPercent;
        }
    }

    public struct OnPrintTemperatureChanged
    {
        public int Temperature { get; }

        public OnPrintTemperatureChanged(int temperature)
        {
            Temperature = temperature;
        }
    }

    public struct OnPrintSafetyLockChanged
    {
        public bool IsPowerToggleBlocked { get; }
        public bool IsFilamentRemovalBlocked { get; }
        public bool IsPlugToggleBlocked { get; }

        public OnPrintSafetyLockChanged(bool isPowerToggleBlocked, bool isFilamentRemovalBlocked, bool isPlugToggleBlocked)
        {
            IsPowerToggleBlocked = isPowerToggleBlocked;
            IsFilamentRemovalBlocked = isFilamentRemovalBlocked;
            IsPlugToggleBlocked = isPlugToggleBlocked;
        }
    }

    public struct OnFinishedModelSpawned
    {
        public GameObject ModelInstance { get; }

        public OnFinishedModelSpawned(GameObject modelInstance)
        {
            ModelInstance = modelInstance;
        }
    }

    public struct OnFinishedModelCollected { }
    public struct OnNextPrintUnlocked { }
    public struct OnPrinterPanelCleared { }
}