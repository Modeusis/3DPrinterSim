using _Project.Scripts.Interactables;

namespace _Project.Scripts.Printer.Door
{
    public class PrinterDoor : InteractableBase<PrinterElement>
    {
        protected override string AnimatorParameterName => "IsOpen";
        protected override PrinterElement StateElement => PrinterElement.Door;
    }
}