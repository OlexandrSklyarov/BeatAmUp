
using System;

namespace Util.Console
{
    public class BaseDebugCommand
    {
        public string  Id => _id;
        public string  Description => _description;
        public string  Format => _format;

        private string _id;
        private string _description;
        private string _format;


        public BaseDebugCommand(string id, string description, string format)
        {
            _id = id;
            _description = description;
            _format = format;
        }
    }


    public class DebugCommand : BaseDebugCommand
    {
        private Action _command;


        public DebugCommand(string id, string description, string format, Action command) 
            : base(id, description, format)
        {
            _command = command;
        }


        public void Invoke() => _command.Invoke();
    }


    public class DebugCommand<T1> : BaseDebugCommand
    {
        private Action<T1> _command;


        public DebugCommand(string id, string description, string format, Action<T1> command) 
            : base(id, description, format)
        {
            _command = command;
        }


        public void Invoke(T1 value) => _command.Invoke(value);
    }
}