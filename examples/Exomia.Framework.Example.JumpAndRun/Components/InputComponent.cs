using Exomia.ECS.Attributes;

namespace Exomia.Framework.Example.JumpAndRun.Components
{
    [EntityComponentConfiguration(PoolSize = 8)]
    sealed class InputComponent
    {
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Jump { get; set; }
    }
}
