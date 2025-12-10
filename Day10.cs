using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace AdventofCode2025
{
	internal static class Day10
	{
		public class Machine
		{
			public readonly bool[] lightsTarget;
			public readonly Button[] buttons;
			public readonly int[] joltages;

			private Regex lightReg = new Regex(@"\[([\.#]+)\]");
			private Regex btnsReg = new Regex(@"(\(([\d,]+)\) )+");
			private Regex joltsReg = new Regex(@"\{[\d,]+\}");

			public Machine(string input)
			{
				Match m = lightReg.Match(input);
				string lightStr = m.Groups[1].Value;
				lightsTarget = new bool[lightStr.Length];
				for (int i = 0; i < lightStr.Length; i++)
				{
					char l = lightStr[i];
					lightsTarget[i] = l == '#';
				}
				m = btnsReg.Match(input);
				buttons = new Button[m.Groups[1].Captures.Count];
				for (int i = 0; i < buttons.Length; i++)
				{
					buttons[i] = new Button(m.Groups[1].Captures[i].Value);
				}

				//string jStr = joltsReg.Match(input).Groups[1].Value;
				//joltages = jStr.Split(',').Select(int.Parse).ToArray();
			}

			public bool[] Press(int b, bool[] lightState)
			{
				return buttons[b].Press(lightState);
			}
		}

		public class Button
		{
			public readonly int[] wiring;
			public Button(String btnStr)
			{
				btnStr = btnStr.Replace("(", "");
				btnStr = btnStr.Replace(")", "");
				wiring = btnStr.Split(',').Select(int.Parse).ToArray();
			}

			public bool[] Press(bool[] lightState)
			{
				bool[] newState = lightState.ToArray();
				foreach (int w in wiring)
				{
					newState[w] = !lightState[w];
				}

				return newState;
			}
		}

		public class MachineState
		{
			public readonly Machine machine;
			public readonly bool[] lights;

			private readonly int buttonToGetHere = -1;
			public MachineState bestChildState = null;

			public MachineState(Machine m)
			{
				machine = m;
				lights = m.lightsTarget.ToArray();
			}

			public MachineState(Machine m, int b, bool[] l) : this(m)
			{
				lights = l;
				buttonToGetHere = b;
			}

			public MachineState PressButton(int b, bool[] clights)
			{
				if (buttonToGetHere == b)
					return null;

				bool[] nlights = machine.Press(b, clights);
				return new MachineState(machine, b, nlights);
			}

			public bool Matches()
			{
				for (int l = 0; l < lights.Length; l++)
				{
					if (lights[l])
						return false;
				}

				return true;
			}

			public long GetRequiredPresses(bool[] buttonsPressed)
			{
				if (Matches())
				{
					bestChildState = this;
					return 0;
				}

				if (buttonsPressed.All(b => b))
					return (long.MaxValue / 2);

				int availToPress = buttonsPressed.Count(b => !b);

				long min = long.MaxValue;
				for (int b = 0; b < machine.buttons.Length; b++)
				{
					if(buttonsPressed[b]) continue;
					bool[] nB = buttonsPressed.ToArray();
					nB[b] = true;
					MachineState s = PressButton(b, lights.ToArray());
					long p = s?.GetRequiredPresses(nB) ?? (long.MaxValue / 2);
					//min = Math.Min(min, p);
					if (p < min)
					{
						min = p;
						bestChildState = s;
					}
				}

				return min+1;
			}
		}

		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			List<Machine> machines = new List<Machine>();
			foreach (string line in lines)
			{
				machines.Add(new Machine(line));
			}

			foreach (Machine m in machines)
			{
				long r = GetFewestClicks(m);
				;
				result += r;
			}

			return result;
		}

		private static long GetFewestClicks(Machine machine)
		{
			MachineState s = new MachineState(machine);

			bool[] btns = new bool[machine.buttons.Length];

			long p = s.GetRequiredPresses(btns);
			return p;
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			foreach (string line in lines)
			{

			}
			return result;
		}
	}
}
