using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
			private Regex joltsReg = new Regex(@"{([\d,]+)}");

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

				string jStr = joltsReg.Match(input).Groups[1].Value;
				joltages = jStr.Split(',').Select(int.Parse).ToArray();
			}

			public bool[] Press(int b, bool[] lightState)
			{
				return buttons[b].Press(lightState);
			}

			public int[] Jolt(int b, int[] joltState)
			{
				return buttons[b].Press(joltState);
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

			public int[] Press(int[] joltState)
			{
				int[] newState = joltState.ToArray();
				foreach (int w in wiring)
				{
					newState[w] += 1;
				}

				return newState;
			}
		}

		public class MachineState
		{
			public readonly Machine machine;
			public readonly bool[] lights;
			public readonly int[] joltages;

			public readonly int[] buttonPressesToHere;

			public readonly int[] maxPressesPerButton;

			public MachineState parentState = null;

			public MachineState(Machine m)
			{
				machine = m;
				lights = m.lightsTarget.ToArray();
				buttonPressesToHere = new int[m.buttons.Length];
				joltages = new int[m.joltages.Length];
				maxPressesPerButton = new int[m.buttons.Length];
				for (var j = 0; j < machine.buttons.Length; j++)
				{
					maxPressesPerButton[j] = int.MaxValue;
					Button b = machine.buttons[j];
					int[] jolts = new int[m.joltages.Length];
					jolts = b.Press(jolts);
					for (int i = 0; i < machine.joltages.Length; i++)
					{
						maxPressesPerButton[j] = jolts[i] > 0 ? Math.Min(maxPressesPerButton[j], machine.joltages[i]) : maxPressesPerButton[j];
					}
				}
			}

			public MachineState(Machine m, int[] b, bool[] l, MachineState pState) : this(m)
			{
				lights = l;
				buttonPressesToHere = b;
				parentState = pState;
			}

			private MachineState(Machine m, int[] btn, int[] jlt, MachineState pState) : this(m)
			{
				buttonPressesToHere = btn;
				parentState = pState;
				joltages = jlt;

				for (var j = 0; j < machine.buttons.Length; j++)
				{
					maxPressesPerButton[j] = int.MaxValue;
					Button b = machine.buttons[j];
					int[] jolts = new int[m.joltages.Length];
					jolts = b.Press(jolts);
					for (int i = 0; i < machine.joltages.Length; i++)
					{
						maxPressesPerButton[j] = jolts[i] > 0 ? Math.Min(maxPressesPerButton[j], machine.joltages[i] - joltages[i]) : maxPressesPerButton[j];
					}
				}
			}

			public MachineState PressButton(int b)
			{
				if (buttonPressesToHere[b] == 1)
					return null;

				bool[] nlights = machine.Press(b, lights);
				int[] nBtn = buttonPressesToHere.ToArray();
				nBtn[b] = 1;
				return new MachineState(machine, nBtn, nlights, this);
			}

			public MachineState Jolt(int b)
			{
				int[] nJolts = machine.Jolt(b, joltages);
				int[] nBtn = buttonPressesToHere.ToArray();
				nBtn[b] += 1;
				return new MachineState(machine, nBtn, nJolts, this);
			}

			public bool Matches()
			{
				return lights.All(t => !t);
			}

			public bool Powered()
			{
				for (var j = 0; j < joltages.Length; j++)
				{
					if (joltages[j] != machine.joltages[j])
						return false;
				}

				return true;
			}

			public int EstLightDistance()
			{
				return buttonPressesToHere.Count(t => t == 1);
			}

			public int EstJoltDistance()
			{
				int total = 0;
				for (var i = 0; i < joltages.Length; i++)
				{
					int d = machine.joltages[i] - joltages[i];
					if (d < 0)
						return -1;

					total += d;
				}

				if (maxPressesPerButton.Max() < 1 && total > 0)
					return -1;

				return total;
			}

			public override string ToString()
			{
				//return string.Join("", buttonPressesToHere.Select(b => b==1 ? "#" : ".")) + ":" + EstLightDistance();
				return joltages.Sum() + " + " + EstJoltDistance() + " | " + maxPressesPerButton.Sum();
			}

			public bool CanStillSucceed()
			{
				for (var i = 0; i < machine.joltages.Length; i++)
				{
					//int max = maxPressesPerButton.Where((m,n) => machine.buttons[n].wiring.Contains(i)).Sum();
					int max = 0;
					for (int n = 0; n < maxPressesPerButton.Length; n++)
					{
						max += machine.buttons[n].wiring.Contains(i) ? maxPressesPerButton[n] : 0;
					}

					if (machine.joltages[i] - joltages[i] > max)
					{
						return false;
					}
				}

				return true;
			}
		}

		internal static long Part1(string input)
		{
			return -1;
			string[] lines = input.Split('\n');
			long result = 0l;
			List<Machine> machines = new List<Machine>();
			foreach (string line in lines)
			{
				machines.Add(new Machine(line));
			}

			int n = 1;
			foreach (Machine m in machines)
			{
				long r = GetFewestClicks(m);
				Console.WriteLine($"{n++}/{lines.Length} => {r}");
				result += r;
			}

			return result;
		}

		private static long GetFewestClicks(Machine machine)
		{
			MachineState s = new MachineState(machine);
			List<MachineState> openStates = new List<MachineState>();
			openStates.Add(s);

			while (openStates.Any())
			{
				MachineState cState = openStates[0];
				openStates.RemoveAt(0);

				if(cState.Powered()) return cState.buttonPressesToHere.Sum();

				Parallel.For(0, cState.buttonPressesToHere.Length, b=>
				{
					MachineState nState = cState.PressButton(b);

					openStates.Add(nState);
				});
			}

			return int.MaxValue;
		}

		private static int tests = 0;

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			List<Machine> machines = new List<Machine>();
			foreach (string line in lines)
			{
				machines.Add(new Machine(line));
			}

			int n = 1;
			foreach (Machine m in machines)
			{
				tests = 0;
				long r = GetFewestJolts(m);
				Console.WriteLine($"{n++}/{lines.Length} => {r} ({tests})");
				result += r;
			}

			return result;
		}

		private static long GetFewestJolts(Machine machine)
		{
			MachineState s = new MachineState(machine);
			List<MachineState> openStates = new List<MachineState>();
			openStates.Add(s);

			while (openStates.Any())
			{
				MachineState cState = openStates[0];
				openStates.RemoveAt(0);

				if (cState.Powered())
				{
					return cState.buttonPressesToHere.Sum();
				}

				cState.EstJoltDistance();

				for(int b = 0; b < cState.buttonPressesToHere.Length; b++)
				{
					for(int a = 0; a < Math.Max(cState.maxPressesPerButton[b] / 3, 1); a++)
					{
						if (cState.maxPressesPerButton[b] <= 0)
						{
							continue;
						}
						MachineState nState = cState.Jolt(b);

						if (nState.EstJoltDistance() > 0 && nState.CanStillSucceed())
						{
							openStates.Add(nState);
							nState.EstJoltDistance();
						}
					}
				}
				openStates.Sort((a,b) => a.EstJoltDistance().CompareTo(b.EstJoltDistance()));
				if(openStates.Any())
					Console.WriteLine($"...{openStates.Count} / {openStates[0].EstJoltDistance()}");
				else
					Console.WriteLine($"...{openStates.Count} / ---");
				tests++;
			}

			return int.MaxValue;
		}
	}
}
