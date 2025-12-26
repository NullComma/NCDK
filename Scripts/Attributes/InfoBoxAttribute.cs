using System;
using UnityEngine;

namespace EnigmaCore
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class InfoBoxAttribute : Attribute
    {
        public string Message { get; private set; }
        public InfoMessageType Type { get; private set; }
        public string VisibleIfMemberName { get; private set; }

        // Construtor básico
        public InfoBoxAttribute(string message, InfoMessageType type = InfoMessageType.Info, string visibleIf = null)
        {
            Message = message;
            Type = type;
            VisibleIfMemberName = visibleIf;
        }

        // Construtor simplificado (só mensagem)
        public InfoBoxAttribute(string message, string visibleIf)
        {
            Message = message;
            Type = InfoMessageType.Info;
            VisibleIfMemberName = visibleIf;
        }
    }
}