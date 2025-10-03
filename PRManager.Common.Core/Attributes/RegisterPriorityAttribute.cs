using PRManager.Common.Core.Models;

namespace PRManager.Common.Core.Attributes;

public class RegisterPriorityAttribute(RegisterPriorities priority) : Attribute
{
    public RegisterPriorities Priority { get; private set; } = priority;
}