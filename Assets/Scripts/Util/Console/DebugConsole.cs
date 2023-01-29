using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util.Console
{
    public class DebugConsole : MonoBehaviour
    {
        private bool _isShowConsole;
        private bool _isShowHelp;
        private string _input;
        private List<object> _commands;
        private Vector2 _scrollView;

        private const float CONSOLE_BOX_HEIGHT = 50f;
        private const float SIDE_OFFSET = 10f;
        private const float ROW_OFFSET = 20f;
        private const float ROW_HEIGHT = 30f;         
        private const float MAX_SCROLL_VIEW_HEIGHT = 300f;   
        private const string START_TEXT = "~$ ";   


        public void Init(InputServices inputProvider)
        {            
            InitCommands();
            ResetTextInput();

            inputProvider.Player.ToggleDebugConsole.started += (ctx) => OnToggle();
            inputProvider.Player.EnterDebugConsole.started += (ctx) => OnEnter();
        }


        private void ResetTextInput() => _input = START_TEXT;


        private void InitCommands()
        {
            var testCommand = new DebugCommand("cmdTest", "this test command", "cmdTest", () =>
            {
                Util.Debug.PrintColor($"input cmd Test command!!!", Color.yellow);
            });

            var testCommand2 = new DebugCommand<int>("value", "this test command 2", "input_int_value <prm>", (v) =>
            {
                Util.Debug.PrintColor($"input value command => {v}", Color.yellow);
            });

            var help = new DebugCommand("help", "Show commands info", "help", () =>
            {
                _isShowHelp = true;
            });

            _commands = new List<object>()
            {
                testCommand,
                testCommand2,
                help
            };
        }


        private void OnToggle() 
        {
            _isShowConsole = !_isShowConsole;

            if (!_isShowConsole) _isShowHelp = false;
        }


        private void OnEnter()
        {
            if (!_isShowConsole) return;
            
            HandleInput();
            ResetTextInput();            
        }
        

        private void OnGUI()
        {
            if (!_isShowConsole) return;

            float y = 0f;

            //help commands info
            if (_isShowHelp)
            {
                var scrollHeight = Mathf.Min(ROW_HEIGHT * _commands.Count, MAX_SCROLL_VIEW_HEIGHT);

                GUI.Box(new Rect(0f, y, Screen.width, scrollHeight), "-- COMMANDS INFO --");

                var viewport = new Rect(0f, 0f, Screen.width - SIDE_OFFSET, ROW_HEIGHT * _commands.Count);
                
                _scrollView = GUI.BeginScrollView(new Rect(0f, y + ROW_OFFSET, Screen.width, scrollHeight * 0.9f), _scrollView, viewport);

                for (int i = 0; i < _commands.Count; i++)
                {
                    var cmd = _commands[i] as BaseDebugCommand;

                    var label = $"{cmd.Format} - {cmd.Description}";

                    var labelRect = new Rect(ROW_OFFSET, ROW_HEIGHT * i, viewport.width - SIDE_OFFSET, ROW_OFFSET);

                    GUI.Label(labelRect, label);
                }

                GUI.EndScrollView();

                y += scrollHeight;
            }

            //console input
            GUI.Box(new Rect(0f, y, Screen.width, CONSOLE_BOX_HEIGHT), "[CONSOLE]");
            GUI.backgroundColor = new Color();

            _input = GUI.TextField(new Rect(SIDE_OFFSET, y + ROW_OFFSET, Screen.width - SIDE_OFFSET, ROW_HEIGHT), _input);
        }


        private void HandleInput()
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                var cmd = _commands[i] as BaseDebugCommand;
                
                if (_input.Contains(cmd.Id))
                {
                    if (cmd is DebugCommand)
                    {
                        (cmd as DebugCommand).Invoke();
                    }
                    else if (cmd is DebugCommand<int>)
                    {
                        var prop = _input.Split(' ');
                        (cmd as DebugCommand<int>).Invoke(int.Parse(prop[1]));
                    }
                }
            }
        }
    }
}