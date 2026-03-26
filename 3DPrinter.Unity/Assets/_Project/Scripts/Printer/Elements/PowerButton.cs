using _Project.Scripts.Interactables;

namespace _Project.Scripts.Printer.Elements
{
    public class PowerButton : InteractableBase<PrinterElement>
    {
        protected override string AnimatorParameterName => "IsActive";
        protected override PrinterElement StateElement => PrinterElement.PowerButton;
    }
}