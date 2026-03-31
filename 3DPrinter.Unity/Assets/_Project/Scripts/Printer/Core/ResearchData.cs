using System;

namespace _Project.Scripts.Printer.Core
{
    public class ResearchData
    {
        public Complexity Complexity { get; set; }
        public float Speed { get; set; }
        public int Temperature { get; set; }
        public float Accuracy { get; set; }
    }
    
    [Serializable]
    public class Complexity
    {
        public ModelType Type { get; set; } = ModelType.Unknown;
        public float Value { get; set; } = 0;
    }
}