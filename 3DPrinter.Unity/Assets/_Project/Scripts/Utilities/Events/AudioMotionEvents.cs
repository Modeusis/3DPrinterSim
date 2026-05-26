namespace _Project.Scripts.Utilities.Events
{
    public struct OnPrintHeadMovementStateChanged
    {
        public bool IsMoving { get; }

        public OnPrintHeadMovementStateChanged(bool isMoving)
        {
            IsMoving = isMoving;
        }
    }

    public struct OnSpoolRotationStateChanged
    {
        public bool IsRotating { get; }

        public OnSpoolRotationStateChanged(bool isRotating)
        {
            IsRotating = isRotating;
        }
    }

    public struct OnHeatPanelMovementStateChanged
    {
        public bool IsMoving { get; }

        public OnHeatPanelMovementStateChanged(bool isMoving)
        {
            IsMoving = isMoving;
        }
    }
}
